using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyPlaylistArchiver.Repository.Entities;

internal class AlbumArtist : BaseEntity
{
    public Guid AlbumId { get; set; }

    [ForeignKey(nameof(AlbumId))]
    public Album? Album { get; set; }

    public Guid ArtistId { get; set; }

    [ForeignKey(nameof(ArtistId))]
    public Artist? Artist { get; set; }

}
