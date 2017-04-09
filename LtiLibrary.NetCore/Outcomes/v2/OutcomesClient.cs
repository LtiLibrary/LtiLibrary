using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.OAuth;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    /// <summary>
    /// Helper class for Outcomes-2 clients.
    /// </summary>
    public static class OutcomesClient
    {
        #region LineItems

        /// <summary>
        /// Delete a particular LineItem.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<ClientResponse> DeleteLineItemAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await DeleteOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret);
        }

        /// <summary>
        /// Delete a particular LineItem with results.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<ClientResponse> DeleteLineItemResultsAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await DeleteOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret);
        }

        /// <summary>
        /// Get a particular LineItem instance without results from the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>If successful, the LineItem specified in the REST endpoint, but without the results property filled in.</returns>
        public static async Task<ClientResponse<LineItem>> GetLineItemAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await GetOutcomeAsync<LineItem>(client, serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemMediaType);
        }

        /// <summary>
        /// Get a particular LineItem instance with results from the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>If successful, the LineItem specified in the REST endpoint, including results.</returns>
        public static async Task<ClientResponse<LineItem>> GetLineItemWithResultsAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await GetOutcomeAsync<LineItem>(client, serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemResultsMediaType);
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
        /// <returns>If successful, the LineItemContainerPage.</returns>
        public static async Task<ClientResponse<LineItemContainerPage>> GetLineItemsAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, int? limit = null, bool? firstPage = null, int? p = null, string activityId = null)
        {
            var servicePageUrl = GetPagingServiceUrl(serviceUrl, limit, firstPage, p, activityId);
            return await GetOutcomeAsync<LineItemContainerPage>(client, servicePageUrl, consumerKey, consumerSecret, LtiConstants.LineItemContainerMediaType);
        }

        /// <summary>
        /// Create a new LineItem instance within the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem container REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="lineItem">The LineItem to create within the server.</param>
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
            string consumerSecret, LineItem lineItem)
        {
            return await PostOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret, lineItem, LtiConstants.LineItemMediaType);
        }

        /// <summary>
        /// Update a particular LineItem instance, but not the results, within the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="lineItem">The LineItem to be updated within the server.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<ClientResponse> PutLineItemAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret, LineItem lineItem)
        {
            return await PutOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret, lineItem, LtiConstants.LineItemMediaType);
        }

        /// <summary>
        /// Update a particular LineItem instance within the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="lineItem">The LineItem to be updated within the server.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<ClientResponse> PutLineItemWithResultsAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret, LineItem lineItem)
        {
            return await PutOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret, lineItem, LtiConstants.LineItemResultsMediaType);
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
        /// <returns>No content is returned.</returns>
        public static async Task<ClientResponse> DeleteResultAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await DeleteOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret);
        }

        /// <summary>
        /// Get a particular LISResult instance from the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LISResult REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>If successful, the LISResult specified in the REST endpoint.</returns>
        public static async Task<ClientResponse<LisResult>> GetResultAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await GetOutcomeAsync<LisResult>(client, serviceUrl, consumerKey, consumerSecret, LtiConstants.LisResultMediaType);
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
        /// <returns>If successful, the ResultContainerPage.</returns>
        public static async Task<ClientResponse<ResultContainerPage>> GetResultsAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, int? limit = null, bool? firstPage = null, int? p = null)
        {
            var servicePageUrl = GetPagingServiceUrl(serviceUrl, limit, firstPage, p);
            return await GetOutcomeAsync<ResultContainerPage>(client, servicePageUrl, consumerKey, consumerSecret, LtiConstants.LisResultContainerMediaType);
        }

        /// <summary>
        /// Create a new LISResult instance within the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LISResult container REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="result">The LISResult to create within the server.</param>
        /// <returns>If successful, the LISResult with @id filled in.</returns>
        public static async Task<ClientResponse<LisResult>> PostResultAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret, LisResult result)
        {
            return await PostOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret, result, LtiConstants.LisResultMediaType);
        }

        /// <summary>
        /// Update a particular LISResult instance within the server.
        /// </summary>
        /// <param name="client">The HttpClient that will be used to process the request.</param>
        /// <param name="serviceUrl">The LISResult REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="result">The LISResult to be updated within the server.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<ClientResponse> PutResultAsync(HttpClient client, string serviceUrl, string consumerKey, string consumerSecret, LisResult result)
        {
            return await PutOutcomeAsync(client, serviceUrl, consumerKey, consumerSecret, result, LtiConstants.LisResultMediaType);
        }

        #endregion

        #region Private Methods

        private static async Task<ClientResponse> DeleteOutcomeAsync(HttpClient client, string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            try
            {
                await SignRequest(client, HttpMethod.Delete, serviceUrl, new StringContent(string.Empty), consumerKey, consumerSecret);

                var outcomeResponse = new ClientResponse();
                try
                {
                    // HttpClient does not send content in a DELETE request. So there is no Content-Type
                    // header. Therefore, all representations of the resource will be deleted.
                    // See https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/outcomes/LineItem/service.html#DELETE
                    using (var response = await client.DeleteAsync(serviceUrl))
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
            string consumerSecret, string contentType) where T : class
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                await SignRequest(client, HttpMethod.Get, serviceUrl, new StringContent(string.Empty), consumerKey, consumerSecret);
                
                var outcomeResponse = new ClientResponse<T>();
                try
                {
                    using (var response = await client.GetAsync(serviceUrl))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            outcomeResponse.Response = await response.DeserializeJsonObjectAsync<T>();
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
            string consumerKey, string consumerSecret, T content, string contentType) where T : class
        {
            try
            {
                var httpContent = new StringContent(content.ToJsonLdString(), Encoding.UTF8, contentType);

                await SignRequest(client, HttpMethod.Post, serviceUrl, httpContent, consumerKey, consumerSecret);

                var outcomeResponse = new ClientResponse<T>();
                try
                {
                    using (var response = await client.PostAsync(serviceUrl, httpContent))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            outcomeResponse.Response = await response.DeserializeJsonObjectAsync<T>();
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
            string consumerSecret, T content, string contentType)
        {
            try
            {
                var httpContent = new StringContent(content.ToJsonLdString(), Encoding.UTF8, contentType);

                await SignRequest(client, HttpMethod.Put, serviceUrl, httpContent, consumerKey, consumerSecret);

                var outcomeResponse = new ClientResponse();
                try
                {
                    using (var response = await client.PutAsync(serviceUrl, httpContent))
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

        private static async Task SignRequest(HttpClient client, HttpMethod method, string serviceUrl, HttpContent content, string consumerKey, string consumerSecret)
        {
            var parameters = new NameValueCollection();
            parameters.AddParameter(OAuthConstants.ConsumerKeyParameter, consumerKey);
            parameters.AddParameter(OAuthConstants.NonceParameter, Guid.NewGuid().ToString());
            parameters.AddParameter(OAuthConstants.SignatureMethodParameter, OAuthConstants.SignatureMethodHmacSha1);
            parameters.AddParameter(OAuthConstants.VersionParameter, OAuthConstants.Version10);

            // Calculate the timestamp
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var timestamp = Convert.ToInt64(ts.TotalSeconds);
            parameters.AddParameter(OAuthConstants.TimestampParameter, timestamp);

            // Calculate the body hash
            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(await content.ReadAsByteArrayAsync());
                var hash64 = Convert.ToBase64String(hash);
                parameters.AddParameter(OAuthConstants.BodyHashParameter, hash64);
            }

            // Calculate the signature
            var signature = OAuthUtility.GenerateSignature(method.Method, new Uri(client.BaseAddress, serviceUrl), parameters, consumerSecret);
            parameters.AddParameter(OAuthConstants.SignatureParameter, signature);

            // Build the Authorization header
            var authorization = new StringBuilder(OAuthConstants.AuthScheme).Append(" ");
            foreach (var key in parameters.AllKeys)
            {
                authorization.AppendFormat("{0}=\"{1}\",", key, WebUtility.UrlEncode(parameters[key]));
            }
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authorization.ToString(0, authorization.Length - 1));
        }

        #endregion
    }
}
