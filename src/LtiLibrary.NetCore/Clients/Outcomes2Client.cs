using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lis.v2;
using Result = LtiLibrary.NetCore.Lis.v2.Result;
using LtiLibrary.NetCore.OAuth;

namespace LtiLibrary.NetCore.Clients
{
    /// <summary>
    /// Helper class for Outcomes-2 clients.
    /// </summary>
    public static class Outcomes2Client
    {
        #region LineItems

        /// <summary>
        /// Delete a particular LineItem.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <returns>No content is returned.</returns>
        public static async Task<ClientResponse> DeleteLineItemAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, SignatureMethod signatureMethod = SignatureMethod.HmacSha1)
        {
            return await DeleteOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret, signatureMethod).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a particular LineItem with results.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <returns>No content is returned.</returns>
        public static async Task<ClientResponse> DeleteLineItemResultsAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, SignatureMethod signatureMethod = SignatureMethod.HmacSha1)
        {
            return await DeleteOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret, signatureMethod).ConfigureAwait(false);
        }

        /// <summary>
        /// Get a particular LineItem instance without results from the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <param name="deserializationErrorHandler">A deserialization error handler. Defaults to null.</param>
        /// <returns>If successful, the LineItem specified in the REST endpoint, but without the results property filled in.</returns>
        public static async Task<ClientResponse<LineItem>> GetLineItemAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, SignatureMethod signatureMethod = SignatureMethod.HmacSha1,
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> deserializationErrorHandler = null)
        {
            return await GetOutcomeAsync<LineItem>
                (
                    client, serviceUrl, consumerKey, consumerSecret, LtiConstants.LisLineItemMediaType, 
                    signatureMethod, deserializationErrorHandler
                )
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get a particular LineItem instance with results from the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <param name="deserializationErrorHandler">A deserialization error handler. Defaults to null.</param>
        /// <returns>If successful, the LineItem specified in the REST endpoint, including results.</returns>
        public static async Task<ClientResponse<LineItem>> GetLineItemWithResultsAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, SignatureMethod signatureMethod = SignatureMethod.HmacSha1,
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> deserializationErrorHandler = null)
        {
            return await GetOutcomeAsync<LineItem>
                (
                    client, serviceUrl, consumerKey, consumerSecret, LtiConstants.LisLineItemResultsMediaType, 
                    signatureMethod, deserializationErrorHandler
                )
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get a paginated list of LineItem resources from the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItemContainer REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="limit">The suggested number of lineitems to include in each page.</param>
        /// <param name="firstPage">True to request the first page of lineitems.</param>
        /// <param name="p">The page (2 to ?) of lineitems requested.</param>
        /// <param name="activityId">If specified, the result set will be filtered to only include lineitems that are associated with this activity.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <param name="deserializationErrorHandler">A deserialization error handler. Defaults to null.</param>
        /// <returns>If successful, the LineItemContainerPage.</returns>
        public static async Task<ClientResponse<LineItemContainerPage>> GetLineItemsAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, int? limit = null, bool? firstPage = null, int? p = null, string activityId = null, SignatureMethod signatureMethod = SignatureMethod.HmacSha1,
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> deserializationErrorHandler = null)
        {
            var servicePageUrl = GetPagingServiceUrl(serviceUrl, limit, firstPage, p, activityId);
            return await GetOutcomeAsync<LineItemContainerPage>
                (
                    client, servicePageUrl, consumerKey, consumerSecret, LtiConstants.LisLineItemContainerMediaType, 
                    signatureMethod, deserializationErrorHandler
                )
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Create a new LineItem instance within the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem container REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="lineItem">The LineItem to create within the server.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <param name="deserializationErrorHandler">A deserialization error handler. Defaults to null.</param>
        /// <returns>If successful, the LineItem with @id and results filled in.</returns>
        /// <remarks>
        /// https://www.imsglobal.org/specs/ltiomv2p0/specification-3
        /// When a line item is created, a result for each user is deemed to be created with a status value of “Initialized”.
        /// Thus, there is no need to actually create a result with a POST request; the first connection to a result may be a
        /// PUT or a GET request.When a line item is created, a result for each user is deemed to be created with a status value 
        /// of “Initialized”.  Thus, there is no need to actually create a result with a POST request; the first connection to a 
        /// result may be a PUT or a GET request.
        /// </remarks>
        public static async Task<ClientResponse<LineItem>> PostLineItemAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, LineItem lineItem, SignatureMethod signatureMethod = SignatureMethod.HmacSha1,
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> deserializationErrorHandler = null)
        {
            return await PostOutcomeAsync
                (
                    client, serviceUrl, consumerKey, consumerSecret, lineItem, LtiConstants.LisLineItemMediaType, 
                    signatureMethod, deserializationErrorHandler
                )
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Update a particular LineItem instance, but not the results, within the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="lineItem">The LineItem to be updated within the server.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <returns>No content is returned.</returns>
        public static async Task<ClientResponse> PutLineItemAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret, 
            LineItem lineItem, SignatureMethod signatureMethod = SignatureMethod.HmacSha1)
        {
            return await PutOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret, lineItem, LtiConstants.LisLineItemMediaType, signatureMethod)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Update a particular LineItem instance within the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="lineItem">The LineItem to be updated within the server.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <returns>No content is returned.</returns>
        public static async Task<ClientResponse> PutLineItemWithResultsAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret, 
            LineItem lineItem, SignatureMethod signatureMethod = SignatureMethod.HmacSha1)
        {
            return await PutOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret, lineItem, LtiConstants.LisLineItemResultsMediaType, signatureMethod)
                .ConfigureAwait(false);
        }

        #endregion

        #region Results

        /// <summary>
        /// Delete a particular LISResult.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LISResult REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <returns>No content is returned.</returns>
        public static async Task<ClientResponse> DeleteResultAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, SignatureMethod signatureMethod = SignatureMethod.HmacSha1)
        {
            return await DeleteOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret, signatureMethod)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get a particular LISResult instance from the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LISResult REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <param name="deserializationErrorHandler">A deserialization error handler. Defaults to null.</param>
        /// <returns>If successful, the LISResult specified in the REST endpoint.</returns>
        public static async Task<ClientResponse<Result>> GetResultAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, SignatureMethod signatureMethod = SignatureMethod.HmacSha1,
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> deserializationErrorHandler = null)
        {
            return await GetOutcomeAsync<Result>
                (
                    client, serviceUrl, consumerKey, consumerSecret, LtiConstants.LisResultMediaType, 
                    signatureMethod, deserializationErrorHandler
                )
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get a paginated list of LISResult resources from the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The ResultContainer REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="limit">The suggested number of results to include in each page.</param>
        /// <param name="firstPage">True to request the first page of results.</param>
        /// <param name="p">The page (2 to ?) of results requested.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <param name="deserializationErrorHandler">A deserialization error handler. Defaults to null.</param>
        /// <returns>If successful, the ResultContainerPage.</returns>
        public static async Task<ClientResponse<ResultContainerPage>> GetResultsAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, int? limit = null, bool? firstPage = null, int? p = null, SignatureMethod signatureMethod = SignatureMethod.HmacSha1,
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> deserializationErrorHandler = null)
        {
            var servicePageUrl = GetPagingServiceUrl(serviceUrl, limit, firstPage, p);
            return await GetOutcomeAsync<ResultContainerPage>
                (
                    client, servicePageUrl, consumerKey, consumerSecret, LtiConstants.LisResultContainerMediaType, 
                    signatureMethod, deserializationErrorHandler
                )
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Create a new LISResult instance within the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LISResult container REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="result">The LISResult to create within the server.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <param name="deserializationErrorHandler">A deserialization error handler. Defaults to null.</param>
        /// <returns>If successful, the LISResult with @id filled in.</returns>
        public static async Task<ClientResponse<Result>> PostResultAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, Result result, SignatureMethod signatureMethod = SignatureMethod.HmacSha1,
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> deserializationErrorHandler = null)
        {
            return await PostOutcomeAsync
                (
                    client, serviceUrl, consumerKey, consumerSecret, result, LtiConstants.LisResultMediaType, 
                    signatureMethod, deserializationErrorHandler
                )
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Update a particular LISResult instance within the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LISResult REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="result">The LISResult to be updated within the server.</param>
        /// <param name="signatureMethod">The signatureMethod. Defaults to <see cref="SignatureMethod.HmacSha1"/></param>
        /// <returns>No content is returned.</returns>
        public static async Task<ClientResponse> PutResultAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret, Result result,
            SignatureMethod signatureMethod = SignatureMethod.HmacSha1)
        {
            return await PutOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret, result, LtiConstants.LisResultMediaType, signatureMethod)
                .ConfigureAwait(false);
        }

        #endregion

        #region Private Methods

        private static async Task<ClientResponse> DeleteOutcomeAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, SignatureMethod signatureMethod)
        {
            try
            {
                HttpRequestMessage webRequest = new HttpRequestMessage(HttpMethod.Delete, serviceUrl);
                await SecuredClient.SignRequest(webRequest, consumerKey, consumerSecret, signatureMethod);

                var outcomeResponse = new ClientResponse();
                try
                {
                    // HttpClient does not send content in a DELETE request. So there is no Content-Type
                    // header. Therefore, all representations of the resource will be deleted.
                    // See https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/outcomes/LineItem/service.html#DELETE
                    using (var response = await client.SendAsync(webRequest))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
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
                return new ClientResponse
                {
                    Exception = ex,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        private static async Task<ClientResponse<T>> GetOutcomeAsync<T>(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, string contentType, SignatureMethod signatureMethod,
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> deserializationErrorHandler) where T : class
        {
            try
            {
                HttpRequestMessage webRequest = new HttpRequestMessage(HttpMethod.Get, serviceUrl);
                webRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
                await SecuredClient.SignRequest(webRequest, consumerKey, consumerSecret, signatureMethod);
                
                var outcomeResponse = new ClientResponse<T>();
                try
                {
                    using (var response = await client.SendAsync(webRequest))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            outcomeResponse.Response = await response.DeserializeJsonObjectAsync<T>(deserializationErrorHandler);
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
                return new ClientResponse<T>
                {
                    Exception = ex,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        private static string GetPagingServiceUrl(string serviceUrl, int? limit, bool? firstPage, int? p, string activityId = null)
        {
            var urlPath = serviceUrl;
            var urlQuery = "?";
            if (serviceUrl.Contains("?"))
            {
                urlPath = serviceUrl.Substring(0, serviceUrl.IndexOf("?", StringComparison.Ordinal));
                urlQuery = serviceUrl.Substring(serviceUrl.IndexOf("?", StringComparison.Ordinal));
            }
            var query = new StringBuilder(urlQuery);
            if (limit != null)
            {
                query.AppendFormat("&limit={0}", limit.Value);
            }
            if (firstPage != null)
            {
                query.Append("&firstPage");
            }
            if (p != null)
            {
                query.AppendFormat("&p={0}", p.Value);
            }
            if (!string.IsNullOrEmpty(activityId))
            {
                query.AppendFormat("&activityId={0}", activityId);
            }

            return $"{urlPath}{query}";
        }

        private static async Task<ClientResponse<T>> PostOutcomeAsync<T>(HttpClient client, string serviceUrl,
            string consumerKey, string consumerSecret, T content, string contentType, SignatureMethod signatureMethod,
            EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> deserializationErrorHandler) where T : class
        {
            try
            {
                HttpRequestMessage webRequest = new HttpRequestMessage(HttpMethod.Post, serviceUrl)
                {
                    Content = new StringContent(content.ToJsonLdString(), Encoding.UTF8, contentType)
                };
                await SecuredClient.SignRequest(webRequest, consumerKey, consumerSecret, signatureMethod);

                var outcomeResponse = new ClientResponse<T>();
                try
                {
                    using (var response = await client.SendAsync(webRequest))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            outcomeResponse.Response = await response.DeserializeJsonObjectAsync<T>(deserializationErrorHandler);
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
                return new ClientResponse<T>
                {
                    Exception = ex,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        private static async Task<ClientResponse> PutOutcomeAsync<T>(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, T content, string contentType, SignatureMethod signatureMethod)
        {
            try
            {
                HttpRequestMessage webRequest = new HttpRequestMessage(HttpMethod.Put, serviceUrl)
                {
                    Content = new StringContent(content.ToJsonLdString(), Encoding.UTF8, contentType)
                };
                await SecuredClient.SignRequest(webRequest, consumerKey, consumerSecret, signatureMethod);

                var outcomeResponse = new ClientResponse();
                try
                {
                    using (var response = await client.SendAsync(webRequest))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
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
                return new ClientResponse
                {
                    Exception = ex,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        #endregion
    }
}
