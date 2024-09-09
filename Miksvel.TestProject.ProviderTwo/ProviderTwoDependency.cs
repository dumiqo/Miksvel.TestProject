using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Miksvel.TestProject.Cache;
using Miksvel.TestProject.Core;

namespace Miksvel.TestProject.ProviderTwo
{
    public class ProviderTwoDependency
    {
        public static void Register(IServiceCollection serviceProvider, IConfiguration configuration)
        {
            serviceProvider.AddAutoMapper(typeof(ProviderTwoProfile));
            serviceProvider.AddScoped<IProviderTwoClient, ProviderTwoClient>();
            serviceProvider.AddScoped<ISearchService, ProviderTwoSearchService>();
            serviceProvider.AddHttpClient();
            serviceProvider.AddOptions<ProviderTwoConfiguration>()
                .Bind(configuration.GetSection("ProviderTwo"))
                .ValidateOnStart();
            CacheDependency.Register(serviceProvider);
        }
    }
}
