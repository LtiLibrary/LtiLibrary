using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lis.v1;
using LtiLibrary.NetCore.Lis.v2;
using LtiLibrary.NetCore.OAuth;

namespace LtiLibrary.NetCore.Clients
{
    /// <summary>
    /// Helper class for Membership clients.
    /// </summary>
    public static class MembershipClient
    {
        /// <summary>
        /// Get the membership of a context. Calls GetMembershipPageAsync until there is no NextPage, and returns all the <see cref="Membership"/> records collected.
        /// </summary>
        /// <param name="client">The HTTP client to use for this request.</param>
        /// <param name="serviceUrl">The membership service URL. In LTI 1 the parameter will be in the launch as <code>custom_context_membership</code>.</param>
        /// <param name="consumerKey">The OAuth Consumer Key.</param>
        /// <param name="consumerSecret">The OAuth Consumer Secret to use for signing the request.</param>
        /// <param name="rlid">The ID of a resource link within the context and associated and the Tool Provider. The result set will be filtered so that it includes only those memberships that are permitted to access the resource link. If omitted, the result set will include all memberships for the context.</param>
        /// <param name="role">The role for a membership. The result set will be filtered so that it includes only those memberships that contain this role. The value of the parameter should be the full URI for the role, although the simple name may be used for context-level roles. If omitted, the result set will include all memberships with any role.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <param name="deserializationErrorHandler">A deserialization error handler. Defaults to null.</param>
        public static async Task<ClientResponse<List<Membership>>> GetMembershipAsync(
            HttpClient client, string serviceUrl, string consumerKey, string consumerSecret,
            string rlid = null, ContextRole? role = null, SignatureMethod signatureMethod = SignatureMethod.HmacSha1,
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> deserializationErrorHandler = null)
        {
            var filteredServiceUrl = GetFilteredServiceUrl(serviceUrl, null, rlid, role);
            var pageResponse = await GetFilteredMembershipPageAsync
                (
                    client, filteredServiceUrl, consumerKey, consumerSecret, 
                    LtiConstants.LisMembershipContainerMediaType, signatureMethod,
                    deserializationErrorHandler
                ).ConfigureAwait(false);

            Uri pageId = null;
            var result = new ClientResponse<List<Membership>>
            {
                Response = new List<Membership>()
            };
            do
            {
#if DEBUG
                result.HttpRequest = pageResponse.HttpRequest;
                result.HttpResponse = pageResponse.HttpResponse;
#endif
                result.StatusCode = pageResponse.StatusCode;
                if (pageResponse.StatusCode != HttpStatusCode.OK)
                {
                    return result;
                }
                if (pageResponse.Response == null)
                {
                    return result;
                }

                // If the same page is comming back from the consumer, just
                // return what we have.
                if (pageId != null && pageId == pageResponse.Response.Id)
                {
                    return result;
                }
                pageId = pageResponse.Response.Id;
                
                // Add the memberships to the list (the collection cannot be null)
                if (pageResponse.Response.MembershipContainer?.MembershipSubject?.Membership != null)
                {
                    result.Response.AddRange(pageResponse.Response.MembershipContainer.MembershipSubject.Membership);
                }
                
                // Repeat until there is no NextPage
                if (string.IsNullOrWhiteSpace(pageResponse.Response.NextPage)) break;

                // Get the next page
                filteredServiceUrl = GetFilteredServiceUrl(pageResponse.Response.NextPage, null, rlid, role);
                pageResponse = await GetFilteredMembershipPageAsync
                    (
                        client, filteredServiceUrl, consumerKey, consumerSecret, 
                        LtiConstants.LisMembershipContainerMediaType, signatureMethod,
                        deserializationErrorHandler
                    ).ConfigureAwait(false);
            } while (true);

            return result;
        }

        /// <summary>
        /// Get a page of membership.
        /// </summary>
        /// <param name="client">The HTTP client to use for this request.</param>
        /// <param name="serviceUrl">The membership service URL. In LTI 1 the parameter will be in the launch as <code>custom_context_membership</code>.</param>
        /// <param name="consumerKey">The OAuth Consumer Key.</param>
        /// <param name="consumerSecret">The OAuth Consumer Secret to use for signing the request.</param>
        /// <param name="limit">Specifies the maximum number of items that should be delivered per page. This parameter is merely a hint. The server is not obligated to honor this limit and may at its own discretion choose a different value for the number of items per page.</param>
        /// <param name="rlid">The ID of a resource link within the context and associated and the Tool Provider. The result set will be filtered so that it includes only those memberships that are permitted to access the resource link. If omitted, the result set will include all memberships for the context.</param>
        /// <param name="role">The role for a membership. The result set will be filtered so that it includes only those memberships that contain this role. The value of the parameter should be the full URI for the role, although the simple name may be used for context-level roles. If omitted, the result set will include all memberships with any role.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <param name="deserializationErrorHandler">A deserialization error handler. Defaults to null.</param>
        public static async Task<ClientResponse<MembershipContainerPage>> GetMembershipPageAsync(
            HttpClient client, string serviceUrl, string consumerKey, string consumerSecret,
            int? limit = null, string rlid = null, ContextRole? role = null, SignatureMethod signatureMethod = SignatureMethod.HmacSha1,
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> deserializationErrorHandler = null)
        {
            var filteredServiceUrl = GetFilteredServiceUrl(serviceUrl, limit, rlid, role);
            return await GetFilteredMembershipPageAsync
                (
                    client, filteredServiceUrl, consumerKey, consumerSecret,
                    LtiConstants.LisMembershipContainerMediaType, signatureMethod,
                    deserializationErrorHandler
                ).ConfigureAwait(false);
        }

        private static async Task<ClientResponse<MembershipContainerPage>> GetFilteredMembershipPageAsync(
            HttpClient client, string serviceUrl, string consumerKey, string consumerSecret,
            string contentType, SignatureMethod signatureMethod,
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> deserializationErrorHandler)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                await SecuredClient.SignRequest(client, HttpMethod.Get, serviceUrl, new StringContent(string.Empty), consumerKey,
                    consumerSecret, signatureMethod);

                var outcomeResponse = new ClientResponse<MembershipContainerPage>();
                try
                {
                    using (var response = await client.GetAsync(serviceUrl).ConfigureAwait(false))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            outcomeResponse.Response = await response.DeserializeJsonObjectAsync<MembershipContainerPage>(deserializationErrorHandler)
                                .ConfigureAwait(false);
                        }
#if DEBUG
                        outcomeResponse.HttpRequest = await response.RequestMessage.ToFormattedRequestStringAsync()
                            .ConfigureAwait(false);
                        outcomeResponse.HttpResponse = await response.ToFormattedResponseStringAsync()
                            .ConfigureAwait(false);
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

        private static string GetFilteredServiceUrl(string serviceUrl, int? limit, string rlid, ContextRole? role)
        {
            var query = new StringBuilder();
            if (limit.HasValue)
            {
                query.Append($"limit={limit}&");
            }
            if (!string.IsNullOrWhiteSpace(rlid))
            {
                query.Append($"rlid={rlid}&");
            }
            if (role.HasValue)
            {
                query.Append($"role={role}&");
            }
            if (query.Length <= 0)
            {
                return serviceUrl;
            }
            query.Remove(query.Length - 1, 1);
            return serviceUrl.Contains("?") ? $"{serviceUrl}&{query}" : $"{serviceUrl}?{query}";
        }
    }
}