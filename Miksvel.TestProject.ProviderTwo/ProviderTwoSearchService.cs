using AutoMapper;
using Miksvel.TestProject.Core;
using Miksvel.TestProject.Core.Model;
using Miksvel.TestProject.ProviderTwo.Model;

namespace Miksvel.TestProject.ProviderTwo
{
    public class ProviderTwoSearchService : ISearchService
    {
        private readonly IProviderTwoClient _providerClient;
        private readonly IMapper _mapper;

        public ProviderTwoSearchService(
            IMapper mapper,
            IProviderTwoClient providerClient)
        {
            _mapper = mapper;
            _providerClient = providerClient;
        }

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            return await _providerClient.IsAvailableAsync(cancellationToken);
        }

        public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            var response = await _providerClient.SearchAsync(_mapper.Map<ProviderTwoSearchRequest>(request), cancellationToken);
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

        private ProviderTwoRoute[] Filter(ProviderTwoRoute[] routes, SearchFilters? filter)
        {
            if (filter == null)
            {
                return routes;
            }

            var enumerable = routes.AsEnumerable();

            if (filter.MaxPrice != null)
            {
                enumerable = enumerable.Where(x => x.Price <= filter.MaxPrice);
            }
            if (filter.DestinationDateTime != null)
            {
                enumerable = enumerable.Where(x => x.Arrival.Date <= filter.DestinationDateTime);
            }

            return enumerable.ToArray();
        }
    }
}
