using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Miksvel.TestProject.Cache;
using Miksvel.TestProject.Core;

namespace Miksvel.TestProject.ProviderOne
{
    public class ProviderOneDependency
    {
        public static void Register(IServiceCollection serviceProvider, IConfiguration configuration)
        {
            serviceProvider.AddAutoMapper(typeof(ProviderOneProfile));
            serviceProvider.AddScoped<IProviderOneClient, ProviderOneClient>();
            serviceProvider.AddScoped<ISearchService, ProviderOneSearchService>();
            serviceProvider.AddHttpClient();
            serviceProvider.AddOptions<ProviderOneConfiguration>()
                .Bind(configuration.GetSection("ProviderOne"))
                .ValidateOnStart();
            CacheDependency.Register(serviceProvider);
        }
    }
}
