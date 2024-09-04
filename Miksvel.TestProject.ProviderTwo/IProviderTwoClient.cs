using Miksvel.TestProject.ProviderTwo.Model;

namespace Miksvel.TestProject.ProviderTwo
{
    public interface IProviderTwoClient
    {
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
        Task<ProviderTwoSearchResponse> SearchAsync(ProviderTwoSearchRequest request, CancellationToken cancellationToken);
    }
}
