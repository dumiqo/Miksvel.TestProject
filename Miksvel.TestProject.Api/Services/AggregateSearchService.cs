using Miksvel.TestProject.Core;
using Miksvel.TestProject.Core.Model;
using Route = Miksvel.TestProject.Core.Model.Route;

namespace Miksvel.TestProject.Api.Services
{
    public class AggregateSearchService : IAggregateSearchService
    {
        public IEnumerable<ISearchService> _searchServices;
        private ILogger<AggregateSearchService> _logger;

        public AggregateSearchService(
            IEnumerable<ISearchService> searchServices, 
            ILogger<AggregateSearchService> logger)
        {
            _searchServices = searchServices;
            _logger = logger;
        }

        public async Task<SearchResponse> SearchAsync(
            SearchRequest request, 
            CancellationToken cancellationToken)
        {
            var searchTasks = _searchServices
                .Select(x => SearchInternalAsync(x, request, cancellationToken))
                .ToArray();

            await Task.WhenAll(searchTasks);
            var results = searchTasks.Select(x => x.Result)
                .Where(x => x != null)
                .ToArray();

            var routes = new List<Route>();
            var minPrice = decimal.MaxValue;
            var maxPrice = decimal.MinValue;
            var minMinutesRoute = int.MaxValue;
            var maxMinutesRoute = int.MinValue;

            foreach (var item in results)
            {
                routes.AddRange(item.Routes);
                minPrice = decimal.Min(minPrice, item.MinPrice);
                maxPrice = decimal.Max(maxPrice, item.MaxPrice);
                minMinutesRoute = int.Min(minMinutesRoute, item.MinMinutesRoute);
                maxMinutesRoute = int.Max(maxMinutesRoute, item.MaxMinutesRoute);
            }

            return new SearchResponse
            {
                Routes = routes.OrderBy(x => x.Price)
                            .ThenByDescending(x => x.OriginDateTime)
                            .ToArray(),
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MaxMinutesRoute = maxMinutesRoute,
                MinMinutesRoute = minMinutesRoute,
            };
        }

        private async Task<SearchResponse?> SearchInternalAsync(
            ISearchService service, 
            SearchRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                //note maybe we don`t need to check availability if use only cache
                var isAvailable = await service.IsAvailableAsync(cancellationToken);
                if (!isAvailable)
                {
                    return null;
                }

                return await service.SearchAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in searching for routes");
                return null;
            }
        } 
    }
}
