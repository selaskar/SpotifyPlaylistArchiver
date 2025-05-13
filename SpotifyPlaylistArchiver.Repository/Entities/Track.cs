using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyPlaylistArchiver.Repository.Entities;

internal class Track : NamedTrackedEntity
{
    public Guid AlbumId { get; set; }

    [ForeignKey(nameof(AlbumId))]
    public Album? Album { get; set; }

    public IList<TrackArtist> Artists { get; set; } = [];
}
