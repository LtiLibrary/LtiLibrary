using LtiLibrary.NetCore.Common;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lti1;

namespace LtiLibrary.NetCore.Lis.v2
{
    /// <summary>
    /// Helper class for Membership clients.
    /// </summary>
    public static class MembershipClient
    {
        /// <summary>
        /// Get the membership list of a context.
        /// </summary>
        /// <param name="client">The HTTP client to use for this request.</param>
        /// <param name="serviceUrl">The membership service URL. In LTI 1 the parameter will be in the launch as <code>custom_context_membership</code>.</param>
        /// <param name="consumerKey">The OAuth Consumer Key.</param>
        /// <param name="consumerSecret">The OAuth Consumer Secret to use for signing the request.</param>
        /// <param name="limit">Specifies the maximum number of items that should be delivered per page. This parameter is merely a hint. The server is not obligated to honor this limit and may at its own discretion choose a different value for the number of items per page.</param>
        /// <param name="rlid">The ID of a resource link within the context and associated and the Tool Provider. The result set will be filtered so that it includes only those memberships that are permitted to access the resource link. If omitted, the result set will include all memberships for the context.</param>
        /// <param name="role">The role for a membership. The result set will be filtered so that it includes only those memberships that contain this role. The value of the parameter should be the full URI for the role, although the simple name may be used for context-level roles. If omitted, the result set will include all memberships with any role.</param>
        /// <returns></returns>
        public static async Task<ClientResponse<MembershipContainerPage>> GetMembershipsAsync(
            HttpClient client, string serviceUrl, string consumerKey, string consumerSecret,
            int? limit = 0, string rlid = null, Role? role = null)
        {
            var filteredServiceUrl = GetFilteredServiceUrl(serviceUrl, limit, rlid, role);
            return await GetFilteredMembershipsAsync(client, filteredServiceUrl, consumerKey, consumerSecret,
                LtiConstants.LisMembershipContainerMediaType);
        }

        private static async Task<ClientResponse<MembershipContainerPage>> GetFilteredMembershipsAsync(
            HttpClient client, string serviceUrl, string consumerKey, string consumerSecret,
            string contentType)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                await SignRequest(client, HttpMethod.Get, serviceUrl, new StringContent(string.Empty), consumerKey,
                    consumerSecret);

                var outcomeResponse = new ClientResponse<MembershipContainerPage>();
                try
                {
                    using (var response = await client.GetAsync(serviceUrl))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            outcomeResponse.Response = await response.DeserializeJsonObjectAsync<MembershipContainerPage>();
                        }
#if DEBUG
                        outcomeResponse.HttpRequest = await response.RequestMessage.ToFormattedRequestStringAsync();
                        outcomeResponse.HttpResponse = await response.ToFormattedResponseStringAsync();
#endif
                    }
                }
                catch (HttpRequestException ex)
                {
                    outcomeResponse.Exception = ex;
                    outcomeResponse.StatusCode = HttpStatusCode.BadRequest;
                }
                catch (Exception ex)
                {
                    outcomeResponse.Exception = ex;
                    outcomeResponse.StatusCode = HttpStatusCode.InternalServerError;
                }
                return outcomeResponse;

            }
            catch (Exception ex)
            {
                return new ClientResponse<MembershipContainerPage>
                {
                    Exception = ex,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        private static string GetFilteredServiceUrl(string serviceUrl, int? limit, string rlid, Role? role)
        {
            var query = new StringBuilder();
            if (limit.HasValue)
            {
                query.Append($"limit={limit.Value}&");
            }
            if (!string.IsNullOrWhiteSpace(rlid))
            {
                query.Append($"rlid={rlid}&");
            }
            if (role.HasValue)
            {
                query.Append($"role={role}&");
            }
            if (query.Length > 0)
            {
                query.Remove(query.Length - 1, 1);
                if (serviceUrl.Contains("?"))
                {
                    return $"{serviceUrl}&{query}";
                }
                else
                {
                    return $"{serviceUrl}?{query}";
                }
            }
            return serviceUrl;
        }

        private static async Task SignRequest(HttpClient client, HttpMethod method, string serviceUrl,
            HttpContent content, string consumerKey, string consumerSecret)
        {
            var ltiRequest = new LtiRequest(LtiConstants.OutcomesMessageType)
            {
                ConsumerKey = consumerKey,
                HttpMethod = method.Method,
                Url = new Uri(client.BaseAddress, serviceUrl)
            };

            AuthenticationHeaderValue authorizationHeader;

            // Create an Authorization header using the body hash
            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(await content.ReadAsByteArrayAsync());
                authorizationHeader = ltiRequest.GenerateAuthorizationHeader(hash, consumerSecret);
            }

            // Attach the header to the client request
            client.DefaultRequestHeaders.Authorization = authorizationHeader;
        }
    }
}