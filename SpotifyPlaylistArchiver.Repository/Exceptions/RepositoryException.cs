namespace SpotifyPlaylistArchiver.Repository.Exceptions;

public class RepositoryException(string message, Exception? inner) : Exception(message, inner)
{
}
