using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;

namespace LtiLibrary.NetCore.Profiles
{
    public static class ToolConsumerProfileClient
    {
        /// <summary>
        /// Get a ToolConsumerProfile from the service endpoint.
        /// </summary>
        /// <param name="client">The HttpClient to use for the request.</param>
        /// <param name="serviceUrl">The full URL of the ToolConsumerProfile service.</param>
        /// <returns>A <see cref="ClientResponse"/> with the <see cref="ToolConsumerProfile"/> successful.</returns>
        public static async Task<ClientResponse<ToolConsumerProfile>> GetToolConsumerProfileAsync(HttpClient client, string serviceUrl)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(LtiConstants.ToolConsumerProfileMediaType));

            var profileResponse = new ClientResponse<ToolConsumerProfile>();
            using (var response = await client.GetAsync(serviceUrl))
            {
                profileResponse.StatusCode = response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    profileResponse.Response =
                        await response.Content.ReadJsonAsObjectAsync<ToolConsumerProfile>();
                }
#if DEBUG
                profileResponse.HttpRequest = await response.RequestMessage.ToFormattedRequestStringAsync();
                profileResponse.HttpResponse = await response.ToFormattedResponseStringAsync();
#endif
            }
            return profileResponse;
        }
    }
}
