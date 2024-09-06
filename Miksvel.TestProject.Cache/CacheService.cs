using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Miksvel.TestProject.Cache.Context;
using Newtonsoft.Json;

namespace Miksvel.TestProject.Cache
{
    public class CacheService<T> : ICacheService<T>
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public CacheService(IServiceScopeFactory scopeFactory)
        {
            this._scopeFactory = scopeFactory;
        }

        public async Task<bool> TryAddAsync(string key, T obj, TimeSpan ttl, CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<CacheDbContext>();
                var cache = await dbContext.Cache.FirstOrDefaultAsync(x => x.Key.Equals(key), cancellationToken);

                var str = JsonConvert.SerializeObject(obj);
                if (cache == null)
                {
                    await dbContext.Cache.AddAsync(new CacheEntry
                    {
                        Key = key,
                        Value = str,
                        ExpiredTime = DateTimeOffset.UtcNow.Add(ttl)
                    }, cancellationToken);
                }
                else
                {
                    cache.Value = str;
                }
                await dbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
        }

        public async Task<IEnumerable<T>?> FindAsync(string keyPattern, CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<CacheDbContext>();
                var cacheEntries = await dbContext.Cache
                .AsNoTracking()
                .Where(x => x.Key.StartsWith(keyPattern) && x.ExpiredTime >= DateTimeOffset.UtcNow)
                .ToArrayAsync(cancellationToken);
                if (cacheEntries == null || cacheEntries.Length <= 0)
                {
                    return null;
                }

                return cacheEntries.Select(x => JsonConvert.DeserializeObject<T>(x.Value));
            }
        }
    }
}
