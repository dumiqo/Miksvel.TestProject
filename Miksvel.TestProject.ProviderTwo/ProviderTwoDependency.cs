using Microsoft.Extensions.DependencyInjection;
using Miksvel.TestProject.Cache;
using Miksvel.TestProject.Core;

namespace Miksvel.TestProject.ProviderTwo
{
    public class ProviderTwoDependency
    {
        public static void Register(IServiceCollection serviceProvider)
        {
            serviceProvider.AddAutoMapper(typeof(ProviderTwoProfile));
            serviceProvider.AddScoped<IProviderTwoClient, ProviderTwoClient>();
            serviceProvider.AddScoped<ISearchService, ProviderTwoSearchService>();
            serviceProvider.AddHttpClient();
            CacheDependency.Register(serviceProvider);
        }
    }
}
