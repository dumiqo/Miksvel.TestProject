namespace Miksvel.TestProject.Cache
{
    public interface ICacheService
    {
        Task<bool> Add<T>(string key, T obj);
        Task<T> Get<T>(string key);
    }
}
