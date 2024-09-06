using Microsoft.AspNetCore.Mvc;
using Miksvel.TestProject.Api.Services;
using Miksvel.TestProject.Core.Model;

namespace Miksvel.TestProject.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IAggregateSearchService _aggregateSearchService;

        public SearchController(IAggregateSearchService aggregateSearchService)
        {
            _aggregateSearchService = aggregateSearchService;
        }

        [HttpPost]
        public async Task<SearchResponse> Search(SearchRequest request)
        {
            return await _aggregateSearchService.SearchAsync(request, default);
        }
    }
}
