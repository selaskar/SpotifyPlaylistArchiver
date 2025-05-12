using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Duende.IdentityModel.Client;
using Duende.IdentityModel.OidcClient;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Maui.Storage;

namespace SpotifyPlaylistArchiver.ViewModels;

public partial class MainViewModel(OidcClient client) : ObservableObject
{
    private string? _currentAccessToken;

    [ObservableProperty]
    private string? message;

    [RelayCommand]
    public async Task Authenticate()
    {
        string cacheDir = Path.Combine(FileSystem.Current.CacheDirectory, "refresh");
        Message = "Login Clicked";

        dynamic result;
        if (!File.Exists(cacheDir))
        {
            result = await client.LoginAsync();
        }
        else
        {
            var refresh = await File.ReadAllTextAsync(cacheDir);
            result = await client.RefreshTokenAsync(refresh);
        }

        if (result.IsError)
        {
            Message = result.Error;
            File.Delete(cacheDir);
            return;
        }

        _currentAccessToken = result.AccessToken;

        var sb = new StringBuilder(128);

        sb.AppendLine("access token:");
        sb.AppendLine(result.AccessToken);

        if (!string.IsNullOrWhiteSpace(result.RefreshToken))
        {
            sb.AppendLine();
            sb.AppendLine("refresh token:");
            sb.AppendLine(result.RefreshToken);
        }

        Message = sb.ToString();

        await File.WriteAllTextAsync(cacheDir, result.RefreshToken);
    }

    [RelayCommand]
    public async Task FetchFavorites()
    {
        Message = "API Clicked";

        if (_currentAccessToken == null)
            return;

        var client = new HttpClient();
        client.SetBearerToken(_currentAccessToken);

        var serializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        int offset = 0, limit = 50;

        TracksResponse? tracksResponse = null;
        SavedTrack[]? savedTracks = null;
        do
        {
            QueryBuilder query = new()
            {
                { "offset", offset.ToString() },
                { "limit", limit.ToString() }
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/me/tracks{query.ToQueryString().Value}");

            var response = await client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                Message = response.ReasonPhrase;
                break;
            }

            var content = await response.Content.ReadAsStringAsync();
            try
            {
                tracksResponse = JsonSerializer.Deserialize<TracksResponse>(content, serializerOptions);

                if (tracksResponse == null)
                    break;

                savedTracks ??= new SavedTrack[tracksResponse.Total];

                Array.Copy(tracksResponse.Items, 0, savedTracks, offset, tracksResponse.Items.Length);
            }
            catch (Exception ex)
            {
                Message = ex.ToString();
                break;
            }

            offset += limit;
        }
        while (tracksResponse.Offset + tracksResponse.Limit < tracksResponse.Total);

        if (savedTracks != null)
            savedTracks = savedTracks.Where(t => !t.Track.IsLocal).ToArray();
    }
}

public class TracksResponse
{
    public int Offset { get; set; }

    public int Limit { get; set; }

    public int Total { get; set; }

    public required SavedTrack[] Items { get; set; }
}

public class SavedTrack
{
    [JsonPropertyName("added_at")]
    public DateTimeOffset AddedAt { get; set; }

    public required Track Track { get; set; }
}

public class Track
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    [JsonPropertyName("is_local")]
    public required bool IsLocal { get; set; }

    public required Album Album { get; set; }

    public required Artist[] Artists { get; set; }
}

public class Album
{
    public required string Id { get; set; }

    public required string Name { get; set; }
}

public class Artist
{
    public required string Id { get; set; }

    public required string Name { get; set; }
}