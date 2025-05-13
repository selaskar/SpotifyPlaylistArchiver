namespace SpotifyPlaylistArchiver.Models;

public class Album
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required Artist[] Artists { get; set; }
}
