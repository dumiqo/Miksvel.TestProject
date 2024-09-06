using Miksvel.TestProject.Cache;
using Miksvel.TestProject.Core.Model;
using Miksvel.TestProject.ProviderOne.Model;
using Newtonsoft.Json;
using System.Text;

namespace Miksvel.TestProject.ProviderOne
{
    public class ProviderOneClient : IProviderOneClient
    {
        private const string ProviderBaseUrl = "http://provider-one/api/v1";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICacheService<ProviderOneRoute> _cache;

        public ProviderOneClient(
            IHttpClientFactory httpClientFactory, 
            ICacheService<ProviderOneRoute> cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{ProviderBaseUrl}/ping", cancellationToken);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<ProviderOneSearchResponse> SearchAsync(
            ProviderOneSearchRequest request,
            bool onlyCache,
            CancellationToken cancellationToken)
        {
            if (onlyCache)
            {
                var routes = await GetFromCacheAsync(request, cancellationToken);
                return new ProviderOneSearchResponse
                {
                    Routes = routes
                };
            }

            var result = await GetFromProviderAsync(request, cancellationToken);
            await AddToCacheAsync(result.Routes, cancellationToken);
            return result;
        }

        private async Task AddToCacheAsync(
            ProviderOneRoute[] routes,
            CancellationToken cancellationToken)
        {
            foreach (var route in routes)
            {
                var ttl = DateTime.Now - route.TimeLimit;
                await _cache.TryAddAsync(GetRouteCacheKey(route), route, ttl, cancellationToken);
            }
        }

        private async Task<ProviderOneRoute[]> GetFromCacheAsync(
            ProviderOneSearchRequest request,
            CancellationToken cancellationToken)
        {
            var key = GetRequestCacheKeyPattern(request);
            var entities = await _cache.FindAsync(key, cancellationToken);

            if (entities == null)
            {
                return [];
            }

            return entities.Where(x => x.DateFrom >= DateTime.UtcNow
                                    && x.DateTo <= request.DateTo
                                    && x.Price <= request.MaxPrice)
                .ToArray();
        }

        private async Task<ProviderOneSearchResponse> GetFromProviderAsync(
            ProviderOneSearchRequest request,
            CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            using var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync($"{ProviderBaseUrl}/search", httpContent, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<ProviderOneSearchResponse>(content);
        }

        private string GetRequestCacheKeyPattern(ProviderOneSearchRequest request)
        {
            return $"{nameof(ProviderOneClient)}-{request.From}-{request.To}";
        }

        private string GetRouteCacheKey(ProviderOneRoute route)
        {
            return $"{nameof(ProviderOneClient)}-{route.From}-{route.To}-{route.DateFrom}";
        }
    }
}
