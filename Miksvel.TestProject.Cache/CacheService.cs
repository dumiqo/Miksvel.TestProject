using Microsoft.EntityFrameworkCore;
using Miksvel.TestProject.Cache.Context;
using Newtonsoft.Json;

namespace Miksvel.TestProject.Cache
{
    public class CacheService<T> : ICacheService<T>
    {
        private readonly CacheDbContext _dbContext;

        public CacheService(CacheDbContext context)
        {
            _dbContext = context;
        }

        public async Task<bool> TryAddAsync(string key, T obj, TimeSpan ttl, CancellationToken cancellationToken)
        {
            var cache = await _dbContext.Cache.FirstOrDefaultAsync(x => x.Key.Equals(key), cancellationToken);

            var str = JsonConvert.SerializeObject(obj);
            if (cache == null)
            {
                await _dbContext.Cache.AddAsync(new CacheEntry {
                    Key = key, 
                    Value = str,
                    ExpiredTime = DateTimeOffset.UtcNow.Add(ttl)
                }, cancellationToken);
            }
            else
            {
                cache.Value = str;
            }
            //todo move to another place, should be triggered only once per request
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IEnumerable<T>?> FindAsync(string keyPattern, CancellationToken cancellationToken)
        {
            var cacheEntries = await _dbContext.Cache
                .AsNoTracking()
                .Where(x => x.Key.StartsWith(keyPattern) && x.ExpiredTime >= DateTimeOffset.Now)
                .ToArrayAsync(cancellationToken);
            if (cacheEntries == null || cacheEntries.Length <= 0)
            {
                return null;
            }

            return cacheEntries.Select(x => JsonConvert.DeserializeObject<T>(x.Value));
        }
    }
}
