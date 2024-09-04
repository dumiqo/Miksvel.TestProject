using Miksvel.TestProject.ProviderTwo.Model;
using Newtonsoft.Json;
using System.Text;

namespace Miksvel.TestProject.ProviderTwo
{
    public class ProviderTwoClient : IProviderTwoClient
    {
        //todo add cache and storing data
        private const string ProviderBaseUrl = "http://provider-two/api/v1";

        private readonly IHttpClientFactory _httpClientFactory;

        public ProviderTwoClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{ProviderBaseUrl}/ping", cancellationToken);

            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<ProviderTwoSearchResponse> SearchAsync(
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
    }
}
