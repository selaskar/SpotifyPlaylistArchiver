using SpotifyPlaylistArchiver.Models;
using SpotifyPlaylistArchiver.Repository.Exceptions;

namespace SpotifyPlaylistArchiver.Repository.Abstract;

public interface ICoreCatalogRepository
{
    /// <exception cref="RepositoryException"/>
    Task SaveCatalog(IEnumerable<SavedTrack> tracks, CancellationToken cancellationToken);
}