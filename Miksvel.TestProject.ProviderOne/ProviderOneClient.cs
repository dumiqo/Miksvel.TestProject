﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Miksvel.TestProject.Cache;
using Miksvel.TestProject.ProviderOne.Model;
using Newtonsoft.Json;
using System.Text;

namespace Miksvel.TestProject.ProviderOne
{
    public class ProviderOneClient : IProviderOneClient
    {
        private readonly ProviderOneConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICacheService<ProviderOneRoute> _cache;
        private readonly ILogger<ProviderOneClient> _logger;

        public ProviderOneClient(
            IOptions<ProviderOneConfiguration> options,
            IHttpClientFactory httpClientFactory,
            ICacheService<ProviderOneRoute> cache,
            ILogger<ProviderOneClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _logger = logger;
            _configuration = options.Value;
        }

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync($"{_configuration.Host}/ping", cancellationToken);

                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in checking livenes of ProviderOne");
                return false;
            }
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
                var ttl = route.TimeLimit - DateTime.Now;
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

            return entities.Where(x => x.DateFrom >= DateTime.Now
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
            var response = await httpClient.PostAsync($"{_configuration.Host}/search", httpContent, cancellationToken);
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
