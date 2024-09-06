using Miksvel.TestProject.Core.Model;

namespace Miksvel.TestProject.Api.Services
{
    public interface IAggregateSearchService
    {
        Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken);
    }
}
