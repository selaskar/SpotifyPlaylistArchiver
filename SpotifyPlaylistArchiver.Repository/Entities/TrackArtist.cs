using System.ComponentModel.DataAnnotations.Schema;

namespace SpotifyPlaylistArchiver.Repository.Entities;

internal class TrackArtist : BaseEntity
{
    public Guid TrackId { get; set; }

    [ForeignKey(nameof(TrackId))]
    public Track? Track { get; set; }

    public Guid ArtistId { get; set; }

    [ForeignKey(nameof(ArtistId))]
    public Artist? Artist { get; set; }

}
