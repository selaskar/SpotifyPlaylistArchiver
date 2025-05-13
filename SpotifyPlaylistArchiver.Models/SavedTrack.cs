using System.Text.Json.Serialization;

namespace SpotifyPlaylistArchiver.Models;

public class SavedTrack
{
    [JsonPropertyName("added_at")]
    public DateTimeOffset AddedAt { get; set; }

    public required Track Track { get; set; }
}
