using System.ComponentModel.DataAnnotations;

namespace SpotifyPlaylistArchiver.Repository.Entities;

internal class NamedTrackedEntity : TrackedEntity
{
    [StringLength(250)]
    public required string Name { get; set; }
}