namespace SpotifyPlaylistArchiver.Repository.Entities;

internal class Album : NamedTrackedEntity
{
    public IList<Track> Tracks { get; set; } = [];

    public IList<AlbumArtist> Artists { get; set; } = [];
}
