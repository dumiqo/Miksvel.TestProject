using AutoMapper;
using Miksvel.TestProject.Core;
using Miksvel.TestProject.Core.Model;
using Miksvel.TestProject.ProviderOne.Model;

namespace Miksvel.TestProject.ProviderOne
{
    public class ProviderOneSearchService : ISearchService
    {
        private readonly IMapper _mapper;
        private readonly IProviderOneClient _providerClient;

        public ProviderOneSearchService(IMapper mapper, IProviderOneClient providerClient)
        {
            _providerClient = providerClient;
            _mapper = mapper;
        }

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            return await _providerClient.IsAvailableAsync(cancellationToken);
        }

        public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            var response = await _providerClient.SearchAsync(_mapper.Map<ProviderOneSearchRequest>(request), cancellationToken);
            var array = Filter(response.Routes, request.Filters);

            var routes = new Route[array.Length];
            var minPrice = decimal.MaxValue;
            var maxPrice = decimal.MinValue;
            var minMinutesRoute = int.MaxValue;
            var maxMinutesRoute = int.MinValue;

            for (int i = 0; i < array.Length; i++)
            {
                routes[i] = _mapper.Map<Route>(array[i]);
                minPrice = decimal.Min(minPrice, routes[i].Price);
                maxPrice = decimal.Max(maxPrice, routes[i].Price);
                var minutesRoute = (int)(routes[i].DestinationDateTime - routes[i].OriginDateTime).TotalMinutes;
                minMinutesRoute = int.Min(minMinutesRoute, minutesRoute);
                maxMinutesRoute = int.Max(maxMinutesRoute, minutesRoute);
            }

            return new SearchResponse
            {
                Routes = routes,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MaxMinutesRoute = maxMinutesRoute,
                MinMinutesRoute = minMinutesRoute,
            };
        }

        private ProviderOneRoute[] Filter(ProviderOneRoute[] routes, SearchFilters? filter)
        {
            if (filter == null)
            {
                return routes;
            }

            var enumerable = routes.AsEnumerable();

            if (filter.MinTimeLimit != null)
            {
                enumerable = enumerable.Where(x => x.TimeLimit >= filter.MinTimeLimit);
            }

            return enumerable.ToArray();
        }
    }
}
