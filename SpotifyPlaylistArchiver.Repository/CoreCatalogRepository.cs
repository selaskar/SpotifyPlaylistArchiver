using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SpotifyPlaylistArchiver.Repository.Abstract;
using SpotifyPlaylistArchiver.Repository.DbContexts;
using SpotifyPlaylistArchiver.Repository.Entities;
using SpotifyPlaylistArchiver.Repository.Exceptions;

namespace SpotifyPlaylistArchiver.Repository;

internal class CoreCatalogRepository(CoreCatalogDbContext dbContext, IMapper mapper) : ICoreCatalogRepository
{
    /// <inheritdoc/>
    public async Task SaveCatalog(IEnumerable<Models.SavedTrack> tracks, CancellationToken cancellationToken)
    {
        IDbContextTransaction? transaction = null;
        try
        {
            transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            List<Guid> trackIds = [];
            foreach (Models.SavedTrack track in tracks)
            {
                Track? trackEntity = await dbContext.Tracks.SingleOrDefaultAsync(t => t.SpotifyId == track.Track.Id && t.RemovedAt == null, cancellationToken);

                if (trackEntity != null)
                {
                    trackEntity.AddedAt = track.AddedAt;
                }
                else
                {
                    trackEntity = mapper.Map<Track>(track);

                    Album albumEntity = await AddOrUpdateAlbum(track.Track.Album, cancellationToken);

                    albumEntity.Tracks.Add(trackEntity);
                    dbContext.Entry(trackEntity).State = EntityState.Added;

                    foreach (Models.Artist artist in track.Track.Artists)
                    {
                        Artist artistEntity = await AddOrUpdateArtist(artist, cancellationToken);

                        await AddTrackArtist(trackEntity.Id, artistEntity.Id, cancellationToken);
                    }
                }

                await dbContext.SaveChangesAsync(cancellationToken);

                trackIds.Add(trackEntity.Id);
            }

            await RemoveMissingTracks(trackIds, cancellationToken);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction?.Rollback();

            if (ex is RepositoryException)
                throw;
            else
                throw new RepositoryException("An error occurred while saving catalog.", ex);
        }
    }

    /// <exception cref="RepositoryException"/>
    private async Task RemoveMissingTracks(List<Guid> trackIds, CancellationToken cancellationToken)
    {
        Track[] missingTracks = await dbContext.Tracks.Where(t => !trackIds.Contains(t.Id)).ToArrayAsync(cancellationToken);

        if (missingTracks.Length == 0)
            return;

        DateTimeOffset now = DateTimeOffset.UtcNow;

        foreach (Track track in missingTracks)
            track.RemovedAt = now;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <exception cref="RepositoryException"/>
    private async Task<Album> AddOrUpdateAlbum(Models.Album album, CancellationToken cancellationToken)
    {
        try
        {
            Album? albumEntity = await dbContext.Albums.SingleOrDefaultAsync(a => a.SpotifyId == album.Id && a.RemovedAt == null, cancellationToken);

            if (albumEntity == null)
            {
                albumEntity = mapper.Map<Album>(album);
                albumEntity.AddedAt = DateTimeOffset.UtcNow;

                await dbContext.Albums.AddAsync(albumEntity, cancellationToken);

                //TODO: artists
                foreach (Models.Artist artist in album.Artists)
                {
                    await AddOrUpdateAlbumArtist(albumEntity.Id, artist, cancellationToken);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return albumEntity;
        }
        catch (Exception ex) when (ex is not RepositoryException)
        {
            throw new RepositoryException($"Failed to add/update album ({album.Id}, {album.Name}).", ex);
        }
    }

    /// <exception cref="RepositoryException"/>
    private async Task<AlbumArtist> AddOrUpdateAlbumArtist(Guid albumId, Models.Artist artist, CancellationToken cancellationToken)
    {
        try
        {
            Artist artistEntity = await AddOrUpdateArtist(artist, cancellationToken);

            AlbumArtist? albumArtistEntity = await dbContext.AlbumArtists.SingleOrDefaultAsync(aa => aa.AlbumId == albumId && aa.ArtistId == artistEntity.Id, cancellationToken);

            if (albumArtistEntity == null)
            {
                albumArtistEntity = new AlbumArtist()
                {
                    Id = Guid.NewGuid(),
                    AlbumId = albumId,
                    ArtistId = artistEntity.Id,
                };

                artistEntity.Albums.Add(albumArtistEntity);
                await dbContext.AlbumArtists.AddAsync(albumArtistEntity, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return albumArtistEntity;
        }
        catch (Exception ex) when (ex is not RepositoryException)
        {
            throw new RepositoryException($"Failed to add/update artist ({artist.Id}, {artist.Name}) for album ({albumId}).", ex);
        }
    }

    /// <exception cref="RepositoryException"/>
    private async Task<Artist> AddOrUpdateArtist(Models.Artist artist, CancellationToken cancellationToken)
    {
        try
        {
            Artist? artistEntity = await dbContext.Artists.SingleOrDefaultAsync(a => a.SpotifyId == artist.Id, cancellationToken);

            if (artistEntity == null)
            {
                artistEntity = mapper.Map<Artist>(artist);
                artistEntity.AddedAt = DateTimeOffset.UtcNow;

                await dbContext.Artists.AddAsync(artistEntity, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            return artistEntity;
        }
        catch (Exception ex)
        {
            throw new RepositoryException($"Failed to add/update artist ({artist.Id}, {artist.Name}).", ex);
        }
    }

    private async Task<TrackArtist> AddTrackArtist(Guid trackId, Guid artistId, CancellationToken cancellationToken)
    {
        TrackArtist trackArtistEntity = new()
        {
            Id = Guid.NewGuid(),
            TrackId = trackId,
            ArtistId = artistId,
        };

        await dbContext.TrackArtists.AddAsync(trackArtistEntity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return trackArtistEntity;
    }
}
