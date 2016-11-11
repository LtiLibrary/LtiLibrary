using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Extensions;
using LtiLibrary.Core.OAuth;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LtiLibrary.Core.Outcomes.v2
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
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<OutcomeResponse> DeleteLineItemAsync(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await DeleteOutcomeAsync(serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemMediaType);
        }

        /// <summary>
        /// Delete a particular LineItem with results.
        /// </summary>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<OutcomeResponse> DeleteLineItemResultsAsync(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await DeleteOutcomeAsync(serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemResultsMediaType);
        }

        /// <summary>
        /// Get a particular LineItem instance without results from the server.
        /// </summary>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>If successful, the LineItem specified in the REST endpoint, but without the results property filled in.</returns>
        public static async Task<OutcomeResponse<LineItem>> GetLineItemAsync(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await GetOutcomeAsync<LineItem>(serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemMediaType);
        }

        /// <summary>
        /// Get a particular LineItem instance with results from the server.
        /// </summary>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>If successful, the LineItem specified in the REST endpoint, including results.</returns>
        public static async Task<OutcomeResponse<LineItem>> GetLineItemWithResultsAsync(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await GetOutcomeAsync<LineItem>(serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemResultsMediaType);
        }

        /// <summary>
        /// Get a paginated list of LineItem resources from the server.
        /// </summary>
        /// <param name="serviceUrl">The LineItemContainer REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="limit">The suggested number of lineitems to include in each page.</param>
        /// <param name="firstPage">True to request the first page of lineitems.</param>
        /// <param name="p">The page (2 to ?) of lineitems requested.</param>
        /// <param name="activityId">If specified, the result set will be filtered to only include lineitems that are associated with this activity.</param>
        /// <returns>If successful, the LineItemContainerPage.</returns>
        public static async Task<OutcomeResponse<LineItemContainerPage>> GetLineItemsAsync(string serviceUrl, string consumerKey,
            string consumerSecret, int? limit = null, bool? firstPage = null, int? p = null, string activityId = null)
        {
            var servicePageUrl = GetPagingServiceUrl(serviceUrl, limit, firstPage, p, activityId);
            return await GetOutcomeAsync<LineItemContainerPage>(servicePageUrl, consumerKey, consumerSecret, LtiConstants.LineItemContainerMediaType);
        }

        /// <summary>
        /// Create a new LineItem instance within the server.
        /// </summary>
        /// <param name="lineItem">The LineItem to create within the server.</param>
        /// <param name="serviceUrl">The LineItem container REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>If successful, the LineItem with @id and results filled in.</returns>
        /// <remarks>
        /// https://www.imsglobal.org/specs/ltiomv2p0/specification-3
        /// When a line item is created, a result for each user is deemed to be created with a status value of “Initialized”.
        /// Thus, there is no need to actually create a result with a POST request; the first connection to a result may be a
        /// PUT or a GET request.When a line item is created, a result for each user is deemed to be created with a status value 
        /// of “Initialized”.  Thus, there is no need to actually create a result with a POST request; the first connection to a 
        /// result may be a PUT or a GET request.
        /// </remarks>
        public static async Task<OutcomeResponse<LineItem>> PostLineItemAsync(LineItem lineItem, string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await PostOutcomeAsync(lineItem, serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemMediaType);
        }

        /// <summary>
        /// Update a particular LineItem instance, but not the results, within the server.
        /// </summary>
        /// <param name="lineItem">The LineItem to be updated within the server.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<OutcomeResponse> PutLineItemAsync(LineItem lineItem, string serviceUrl, string consumerKey, string consumerSecret)
        {
            return await PutOutcomeAsync(lineItem, serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemMediaType);
        }

        /// <summary>
        /// Update a particular LineItem instance within the server.
        /// </summary>
        /// <param name="lineItem">The LineItem to be updated within the server.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<OutcomeResponse> PutLineItemWithResultsAsync(LineItem lineItem, string serviceUrl, string consumerKey, string consumerSecret)
        {
            return await PutOutcomeAsync(lineItem, serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemResultsMediaType);
        }

        #endregion

        #region Results

        /// <summary>
        /// Delete a particular LISResult.
        /// </summary>
        /// <param name="serviceUrl">The LISResult REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<OutcomeResponse> DeleteResultAsync(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await DeleteOutcomeAsync(serviceUrl, consumerKey, consumerSecret);
        }

        /// <summary>
        /// Get a particular LISResult instance from the server.
        /// </summary>
        /// <param name="serviceUrl">The LISResult REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>If successful, the LISResult specified in the REST endpoint.</returns>
        public static async Task<OutcomeResponse<LisResult>> GetResultAsync(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await GetOutcomeAsync<LisResult>(serviceUrl, consumerKey, consumerSecret, LtiConstants.LisResultMediaType);
        }

        /// <summary>
        /// Get a paginated list of LISResult resources from the server.
        /// </summary>
        /// <param name="serviceUrl">The ResultContainer REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <param name="limit">The suggested number of results to include in each page.</param>
        /// <param name="firstPage">True to request the first page of results.</param>
        /// <param name="p">The page (2 to ?) of results requested.</param>
        /// <returns>If successful, the ResultContainerPage.</returns>
        public static async Task<OutcomeResponse<ResultContainerPage>> GetResultsAsync(string serviceUrl, string consumerKey,
            string consumerSecret, int? limit = null, bool? firstPage = null, int? p = null)
        {
            var servicePageUrl = GetPagingServiceUrl(serviceUrl, limit, firstPage, p);
            return await GetOutcomeAsync<ResultContainerPage>(servicePageUrl, consumerKey, consumerSecret, LtiConstants.LisResultContainerMediaType);
        }

        /// <summary>
        /// Create a new LISResult instance within the server.
        /// </summary>
        /// <param name="result">The LISResult to create within the server.</param>
        /// <param name="serviceUrl">The LISResult container REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>If successful, the LISResult with @id filled in.</returns>
        public static async Task<OutcomeResponse<LisResult>> PostResultAsync(LisResult result, string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await PostOutcomeAsync(result, serviceUrl, consumerKey, consumerSecret, LtiConstants.LisResultMediaType);
        }

        /// <summary>
        /// Update a particular LISResult instance within the server.
        /// </summary>
        /// <param name="result">The LISResult to be updated within the server.</param>
        /// <param name="serviceUrl">The LISResult REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<OutcomeResponse> PutResultAsync(LisResult result, string serviceUrl, string consumerKey, string consumerSecret)
        {
            return await PutOutcomeAsync(result, serviceUrl, consumerKey, consumerSecret, LtiConstants.LisResultMediaType);
        }

        #endregion

        #region Private Methods

        private static async Task<OutcomeResponse> DeleteOutcomeAsync(string serviceUrl, string consumerKey,
            string consumerSecret, string contentType = null)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, serviceUrl);
                // Content-Type header is not required. If not specified, all representations of
                // the resource will be deleted.
                // See https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/outcomes/LineItem/service.html#DELETE
                if (!string.IsNullOrWhiteSpace(contentType))
                {
                    request.Content = new StringContent(string.Empty);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }

                SignRequest(request, null, consumerKey, consumerSecret);

                var outcomeResponse = new OutcomeResponse();
                try
                {
#if DEBUG
                    // Capture the request in human readable form for debugging
                    // and inspection while learning
                    outcomeResponse.HttpRequest = await request.ToFormattedRequestStringAsync();
#endif
                    using (var response = await request.GetResponseAsync())
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
#if DEBUG
                        // Capture the response in human readable form for debugging
                        // and inspection while learning
                        outcomeResponse.HttpResponse = response.ToFormattedResponseString(null);
                    }
#endif
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
                return new OutcomeResponse
                {
                    Exception = ex,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        private static async Task<OutcomeResponse<T>> GetOutcomeAsync<T>(string serviceUrl, string consumerKey,
            string consumerSecret, string contentType) where T : class
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, serviceUrl);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                SignRequest(request, null, consumerKey, consumerSecret);
                
                var outcomeResponse = new OutcomeResponse<T>();
                try
                {
#if DEBUG
                    // Capture the request in human readable form for debugging
                    // and inspection while learning
                    outcomeResponse.HttpRequest = await request.ToFormattedRequestStringAsync();
#endif
                    using (var response = await request.GetResponseAsync(allowAutoRedirect: true))
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            outcomeResponse.Outcome = response.DeserializeObject<T>();
                        }
#if DEBUG
                        // Capture the response in human readable form for debugging
                        // and inspection while learning
                        outcomeResponse.HttpResponse = response.ToFormattedResponseString(
                            outcomeResponse.Outcome?.ToJsonString());
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
                return new OutcomeResponse<T>
                {
                    Exception = ex,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        private static string GetPagingServiceUrl(string serviceUrl, int? limit, bool? firstPage, int? p, string activityId = null)
        {
            var uri = new UriBuilder(serviceUrl);
            var query = new StringBuilder(uri.Query);
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
            uri.Query = query.ToString();
            return uri.Uri.AbsoluteUri;
        }

        private static async Task<OutcomeResponse<T>> PostOutcomeAsync<T>(T outcome, string serviceUrl,
            string consumerKey, string consumerSecret, string contentType) where T : class
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, serviceUrl);

                var body = Encoding.UTF8.GetBytes(outcome.ToJsonLdString());
                request.Content = new ByteArrayContent(body);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                SignRequest(request, body, consumerKey, consumerSecret);

                var outcomeResponse = new OutcomeResponse<T>();
                try
                {
#if DEBUG
                    // Capture the request in human readable form for debugging
                    // and inspection while learning
                    outcomeResponse.HttpRequest = await request.ToFormattedRequestStringAsync();
#endif
                    using (var response = await request.GetResponseAsync())
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            outcomeResponse.Outcome = response.DeserializeObject<T>();
                        }
#if DEBUG
                        // Capture the response in human readable form for debugging
                        // and inspection while learning
                        outcomeResponse.HttpResponse = response.ToFormattedResponseString(
                            outcomeResponse.Outcome?.ToJsonString());
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
                return new OutcomeResponse<T>
                {
                    Exception = ex,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        private static async Task<OutcomeResponse> PutOutcomeAsync<T>(T outcome, string serviceUrl, string consumerKey,
            string consumerSecret, string contentType)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Put, serviceUrl);

                var body = Encoding.UTF8.GetBytes(outcome.ToJsonLdString());
                request.Content = new ByteArrayContent(body);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                SignRequest(request, body, consumerKey, consumerSecret);

                var outcomeResponse = new OutcomeResponse();
                try
                {
#if DEBUG
                    // Capture the request in human readable form for debugging
                    // and inspection while learning
                    outcomeResponse.HttpRequest = await request.ToFormattedRequestStringAsync();
#endif
                    using (var response = await request.GetResponseAsync())
                    {
                        outcomeResponse.StatusCode = response.StatusCode;
#if DEBUG
                        // Capture the response in human readable form for debugging
                        // and inspection while learning
                        outcomeResponse.HttpResponse = response.ToFormattedResponseString(null);
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
                return new OutcomeResponse
                {
                    Exception = ex,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        private static void SignRequest(HttpRequestMessage request, byte[] body, string consumerKey, string consumerSecret)
        {
            if (body == null)
                body = new byte[0];
            if (body.Length > 0 && request.Content.Headers.ContentLength != body.Length)
            {
                throw new ArgumentException("body length does not match request.ContentLength", "body");
            }

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
            using (var sha1 = PlatformSpecific.GetSha1Provider())
            {
                var hash = sha1.ComputeHash(body);
                var hash64 = Convert.ToBase64String(hash);
                parameters.AddParameter(OAuthConstants.BodyHashParameter, hash64);
            }

            // Calculate the signature
            var signature = OAuthUtility.GenerateSignature(request.Method.Method.ToUpper(), request.RequestUri, parameters,
                consumerSecret);
            parameters.AddParameter(OAuthConstants.SignatureParameter, signature);

            // Build the Authorization header
            var authorization = new StringBuilder(OAuthConstants.AuthScheme).Append(" ");
            foreach (var key in parameters.AllKeys)
            {
                authorization.AppendFormat("{0}=\"{1}\",", key, WebUtility.UrlEncode(parameters[key]));
            }
            request.Headers.Add(OAuthConstants.AuthorizationHeader, authorization.ToString(0, authorization.Length - 1));
        }

        #endregion
    }
}
