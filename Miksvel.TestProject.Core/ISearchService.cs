using Miksvel.TestProject.Core.Model;

namespace Miksvel.TestProject.Core
{
    public interface ISearchService
    {
        Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken);
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
    }
}
