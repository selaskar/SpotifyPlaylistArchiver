using System.Text.Json.Serialization;

namespace SpotifyPlaylistArchiver.Models;

public class Track
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    [JsonPropertyName("is_local")]
    public required bool IsLocal { get; set; }

    public required Album Album { get; set; }

    public required Artist[] Artists { get; set; }
}
