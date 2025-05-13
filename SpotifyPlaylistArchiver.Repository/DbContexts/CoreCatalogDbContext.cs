using Microsoft.EntityFrameworkCore;
using SpotifyPlaylistArchiver.Repository.Entities;

namespace SpotifyPlaylistArchiver.Repository.DbContexts;

public sealed class CoreCatalogDbContext(DbContextOptions<CoreCatalogDbContext> options) : DbContext(options)
{
    internal DbSet<Album> Albums { get; set; }

    internal DbSet<AlbumArtist> AlbumArtists { get; set; }

    internal DbSet<Artist> Artists { get; set; }

    internal DbSet<Track> Tracks { get; set; }

    internal DbSet<TrackArtist> TrackArtists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AlbumArtist>().ToTable(nameof(AlbumArtist));

        modelBuilder.Entity<TrackArtist>().ToTable(nameof(TrackArtist));
    }
}
