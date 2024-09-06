using Miksvel.TestProject.Cache;
using Miksvel.TestProject.ProviderTwo.Model;
using Newtonsoft.Json;
using System.Text;

namespace Miksvel.TestProject.ProviderTwo
{
    public class ProviderTwoClient : IProviderTwoClient
    {
        private const string ProviderBaseUrl = "http://provider-two/api/v1";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICacheService<ProviderTwoRoute> _cache;

        public ProviderTwoClient(
            IHttpClientFactory httpClientFactory, 
            ICacheService<ProviderTwoRoute> cache)
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

        public async Task<ProviderTwoSearchResponse> SearchAsync(
            ProviderTwoSearchRequest request, 
            bool onlyCache,
            CancellationToken cancellationToken)
        {
            if (onlyCache)
            {
                var routes = await GetFromCacheAsync(request, cancellationToken);
                return new ProviderTwoSearchResponse
                {
                    Routes = routes
                };
            }

            var result = await GetFromProviderAsync(request, cancellationToken);
            await AddToCacheAsync(result.Routes, cancellationToken);
            return result;
        }

        private async Task AddToCacheAsync(
            ProviderTwoRoute[] routes,
            CancellationToken cancellationToken)
        {
            foreach (var route in routes)
            {
                var ttl = DateTime.Now - route.TimeLimit;
                await _cache.TryAddAsync(GetRouteCacheKey(route), route, ttl, cancellationToken);
            }
        }

        private async Task<ProviderTwoRoute[]> GetFromCacheAsync(
            ProviderTwoSearchRequest request,
            CancellationToken cancellationToken)
        {
            var key = GetRequestCacheKeyPattern(request);
            var entities = await _cache.FindAsync(key, cancellationToken);

            if (entities == null)
            {
                return [];
            }

            return entities.Where(x => x.Departure.Date >= DateTime.UtcNow 
                                    && x.TimeLimit >= request.MinTimeLimit)
                .ToArray();
        }

        private async Task<ProviderTwoSearchResponse> GetFromProviderAsync(
            ProviderTwoSearchRequest request,
            CancellationToken cancellationToken)
        {
            var json = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            using var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync($"{ProviderBaseUrl}/search", httpContent, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<ProviderTwoSearchResponse>(content);
        }

        private string GetRequestCacheKeyPattern(ProviderTwoSearchRequest request)
        {
            return $"{nameof(ProviderTwoClient)}-{request.Departure}-{request.Arrival}";
        }

        private string GetRouteCacheKey(ProviderTwoRoute route)
        {
            return $"{nameof(ProviderTwoClient)}-{route.Departure}-{route.Arrival}-{route.Departure.Date}";
        }
    }
}
