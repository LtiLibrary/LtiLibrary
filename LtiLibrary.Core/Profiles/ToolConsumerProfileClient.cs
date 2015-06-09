using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Extensions;

namespace LtiLibrary.Core.Profiles
{
    public static class ToolConsumerProfileClient
    {
        /// <summary>
        /// Get a ToolConsumerProfile from the service endpoint.
        /// </summary>
        /// <param name="serviceUrl">The full URL of the ToolConsumerProfile service.</param>
        /// <returns>A <see cref="ToolConsumerProfileResponse"/> which includes both the HTTP status code
        /// and the <see cref="ToolConsumerProfile"/> if the HTTP status is a success code.</returns>
        public static async Task<ToolConsumerProfileResponse> GetToolConsumerProfile(string serviceUrl)
        {
            var profileResponse = new ToolConsumerProfileResponse();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(LtiConstants.ToolConsumerProfileMediaType));

                var response = await client.GetAsync(serviceUrl);
                profileResponse.StatusCode = response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    profileResponse.ToolConsumerProfile = await response.Content.ReadJsonAsObjectAsync<ToolConsumerProfile>();
                }
            }
            return profileResponse;
        }
    }
}
