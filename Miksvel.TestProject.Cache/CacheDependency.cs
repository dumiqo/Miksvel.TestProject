using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Miksvel.TestProject.Cache.Context;

namespace Miksvel.TestProject.Cache
{
    public class CacheDependency
    {
        public static void Register(IServiceCollection serviceProvider)
        {
            serviceProvider.AddScoped(typeof(ICacheService<>), typeof(CacheService<>));
            serviceProvider.AddDbContext<CacheDbContext>(
                options => options.UseInMemoryDatabase("CacheDb"));
        }
    }
}
