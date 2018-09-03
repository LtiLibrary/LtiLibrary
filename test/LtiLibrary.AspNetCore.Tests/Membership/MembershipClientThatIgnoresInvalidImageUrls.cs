using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Clients;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lis.v2;
using LtiLibrary.NetCore.OAuth;
using Newtonsoft.Json.Serialization;

namespace LtiLibrary.AspNetCore.Tests.Membership
{
    /// <summary>
    /// Helper class for Membership clients.
    /// </summary>
    public static class MembershipClientThatIgnoresInvalidImageUrls
    {
        /// <summary>
        /// Get the membership of a context. Ignore invalid Person.Image URLs.
        /// </summary>
        /// <param name="client">The HTTP client to use for this request.</param>
        /// <param name="serviceUrl">The membership service URL. In LTI 1 the parameter will be in the launch as <code>custom_context_membership</code>.</param>
        /// <param name="consumerKey">The OAuth Consumer Key.</param>
        /// <param name="consumerSecret">The OAuth Consumer Secret to use for signing the request.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        public static async Task<ClientResponse<List<NetCore.Lis.v2.Membership>>> GetMembershipAsync(
            HttpClient client, string serviceUrl, string consumerKey, string consumerSecret, SignatureMethod signatureMethod = SignatureMethod.HmacSha1)
        {
            var pageResponse = await GetMembershipPageAsync(
                client, serviceUrl, consumerKey, consumerSecret, 
                LtiConstants.LisMembershipContainerMediaType, signatureMethod)
                .ConfigureAwait(false);

            Uri pageId = null;
            var result = new ClientResponse<List<NetCore.Lis.v2.Membership>>
            {
                Response = new List<NetCore.Lis.v2.Membership>()
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
                pageResponse = await GetMembershipPageAsync(
                        client, pageResponse.Response.NextPage, consumerKey, consumerSecret, 
                        LtiConstants.LisMembershipContainerMediaType, signatureMethod)
                    .ConfigureAwait(false);
            } while (true);

            return result;
        }

        private static async Task<ClientResponse<MembershipContainerPage>> GetMembershipPageAsync(
            HttpClient client, string serviceUrl, string consumerKey, string consumerSecret,
            string contentType, SignatureMethod signatureMethod)
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
                            outcomeResponse.Response = await response.DeserializeJsonObjectAsync<MembershipContainerPage>(HandleDeserializationError)
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

        // Ignore deserialization errors
        private static void HandleDeserializationError(object sender, ErrorEventArgs e)
        {
            e.ErrorContext.Handled = true;
        }
    }
}
