using System.ComponentModel.DataAnnotations;

namespace SpotifyPlaylistArchiver.Repository.Options;

public class CoreCatalogOptions
{
    public const string Section = "CoreCatalog";

    [Required]
    public required string ConnectionString { get; set; }
}
