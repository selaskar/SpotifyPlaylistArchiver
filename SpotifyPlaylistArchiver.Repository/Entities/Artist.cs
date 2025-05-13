namespace SpotifyPlaylistArchiver.Repository.Entities;

internal class Artist : NamedTrackedEntity
{
    public IList<AlbumArtist> Albums { get; set; } = [];
}
