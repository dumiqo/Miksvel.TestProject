using Miksvel.TestProject.ProviderOne.Model;

namespace Miksvel.TestProject.ProviderOne
{
    public interface IProviderOneClient
    {
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
        Task<ProviderOneSearchResponse> SearchAsync(ProviderOneSearchRequest request, CancellationToken cancellationToken);
    }
}
