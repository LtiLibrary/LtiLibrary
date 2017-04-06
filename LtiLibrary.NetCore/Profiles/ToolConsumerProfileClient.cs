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
        /// <returns>A <see cref="ToolConsumerProfileResponse"/> which includes both the HTTP status code
        /// and the <see cref="ToolConsumerProfile"/> if the HTTP status is a success code.</returns>
        public static async Task<ToolConsumerProfileResponse> GetToolConsumerProfileAsync(HttpClient client, string serviceUrl)
        {
            var profileResponse = new ToolConsumerProfileResponse();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(LtiConstants.ToolConsumerProfileMediaType));

            using (var response = await client.GetAsync(serviceUrl))
            {
                profileResponse.StatusCode = response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    profileResponse.ToolConsumerProfile =
                        await response.Content.ReadJsonAsObjectAsync<ToolConsumerProfile>();
                    profileResponse.ContentType = response.Content.Headers.ContentType.MediaType;
                }
                return profileResponse;
            }
        }
    }
}
