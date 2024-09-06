namespace Miksvel.TestProject.Cache
{
    public interface ICacheService<T> 
    {
        Task<bool> TryAddAsync(string key, T obj, TimeSpan ttl, CancellationToken cancellationToken);
        Task<IEnumerable<T>?> FindAsync(string keyPattern, CancellationToken cancellationToken);
    }
}
