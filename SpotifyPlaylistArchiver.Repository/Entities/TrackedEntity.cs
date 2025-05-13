using System.ComponentModel.DataAnnotations;

namespace SpotifyPlaylistArchiver.Repository.Entities;

internal class TrackedEntity : BaseEntity
{
    [StringLength(25)]
    public required string SpotifyId { get; set; }

    public DateTimeOffset AddedAt { get; set; }

    public DateTimeOffset? RemovedAt { get; set; }
}
