using Microsoft.Extensions.DependencyInjection;
using Miksvel.TestProject.Cache;
using Miksvel.TestProject.Core;

namespace Miksvel.TestProject.ProviderOne
{
    public class ProviderOneDependency
    {
        public static void Register(IServiceCollection serviceProvider)
        {
            serviceProvider.AddAutoMapper(typeof(ProviderOneProfile));
            serviceProvider.AddScoped<IProviderOneClient, ProviderOneClient>();
            serviceProvider.AddScoped<ISearchService, ProviderOneSearchService>();
            serviceProvider.AddHttpClient();
            CacheDependency.Register(serviceProvider);
        }
    }
}
