using System;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
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
        public static async Task<OutcomeResponse> DeleteLineItem(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await DeleteOutcome(serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemMediaType);
        }

        /// <summary>
        /// Get a particular LineItem instance without results from the server.
        /// </summary>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>If successful, the LineItem specified in the REST endpoint, but without the results property filled in.</returns>
        public static async Task<OutcomeResponse<LineItem>> GetLineItem(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await GetOutcome<LineItem>(serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemMediaType);
        }

        /// <summary>
        /// Get a particular LineItem instance with results from the server.
        /// </summary>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>If successful, the LineItem specified in the REST endpoint, including results.</returns>
        public static async Task<OutcomeResponse<LineItem>> GetLineItemWithResults(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await GetOutcome<LineItem>(serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemResultsMediaType);
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
        public static async Task<OutcomeResponse<LineItemContainerPage>> GetLineItems(string serviceUrl, string consumerKey,
            string consumerSecret, int? limit = null, bool? firstPage = null, int? p = null, string activityId = null)
        {
            var servicePageUrl = GetPagingServiceUrl(serviceUrl, limit, firstPage, p, activityId);
            return await GetOutcome<LineItemContainerPage>(servicePageUrl, consumerKey, consumerSecret, LtiConstants.LineItemContainerMediaType);
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
        public static async Task<OutcomeResponse<LineItem>> PostLineItem(LineItem lineItem, string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await PostOutcome(lineItem, serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemMediaType);
        }

        /// <summary>
        /// Update a particular LineItem instance, but not the results, within the server.
        /// </summary>
        /// <param name="lineItem">The LineItem to be updated within the server.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<OutcomeResponse> PutLineItem(LineItem lineItem, string serviceUrl, string consumerKey, string consumerSecret)
        {
            return await PutOutcome(lineItem, serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemMediaType);
        }

        /// <summary>
        /// Update a particular LineItem instance within the server.
        /// </summary>
        /// <param name="lineItem">The LineItem to be updated within the server.</param>
        /// <param name="serviceUrl">The LineItem REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<OutcomeResponse> PutLineItemWithResults(LineItem lineItem, string serviceUrl, string consumerKey, string consumerSecret)
        {
            return await PutOutcome(lineItem, serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemResultsMediaType);
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
        public static async Task<OutcomeResponse> DeleteResult(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await DeleteOutcome(serviceUrl, consumerKey, consumerSecret, LtiConstants.LisResultMediaType);
        }

        /// <summary>
        /// Get a particular LISResult instance from the server.
        /// </summary>
        /// <param name="serviceUrl">The LISResult REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>If successful, the LISResult specified in the REST endpoint.</returns>
        public static async Task<OutcomeResponse<LisResult>> GetResult(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await GetOutcome<LisResult>(serviceUrl, consumerKey, consumerSecret, LtiConstants.LisResultMediaType);
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
        public static async Task<OutcomeResponse<ResultContainerPage>> GetResults(string serviceUrl, string consumerKey,
            string consumerSecret, int? limit = null, bool? firstPage = null, int? p = null)
        {
            var servicePageUrl = GetPagingServiceUrl(serviceUrl, limit, firstPage, p);
            return await GetOutcome<ResultContainerPage>(servicePageUrl, consumerKey, consumerSecret, LtiConstants.LisResultContainerMediaType);
        }

        /// <summary>
        /// Create a new LISResult instance within the server.
        /// </summary>
        /// <param name="result">The LISResult to create within the server.</param>
        /// <param name="serviceUrl">The LISResult container REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>If successful, the LISResult with @id filled in.</returns>
        public static async Task<OutcomeResponse<LisResult>> PostResult(LisResult result, string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await PostOutcome(result, serviceUrl, consumerKey, consumerSecret, LtiConstants.LisResultMediaType);
        }

        /// <summary>
        /// Update a particular LISResult instance within the server.
        /// </summary>
        /// <param name="result">The LISResult to be updated within the server.</param>
        /// <param name="serviceUrl">The LISResult REST endpoint.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to form the Authorization header.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to form the Authorization header.</param>
        /// <returns>No content is returned.</returns>
        public static async Task<OutcomeResponse> PutResult(LisResult result, string serviceUrl, string consumerKey, string consumerSecret)
        {
            return await PutOutcome(result, serviceUrl, consumerKey, consumerSecret, LtiConstants.LisResultMediaType);
        }

        #endregion

        #region Private Methods

        private static async Task<OutcomeResponse> DeleteOutcome(string serviceUrl, string consumerKey,
            string consumerSecret, string contentType = null)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, serviceUrl);
                if (!string.IsNullOrWhiteSpace(contentType))
                {
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                }

                SignRequest(request, null, consumerKey, consumerSecret);

                var outcomeResponse = new OutcomeResponse();
                HttpResponseMessage response = null;
                try
                {
                    response = await request.GetResponseAsync();
                    outcomeResponse.StatusCode = response.StatusCode;
                }
                catch (HttpRequestException)
                {
                    outcomeResponse.StatusCode = response.StatusCode;
                }
                catch (Exception)
                {
                    outcomeResponse.StatusCode = HttpStatusCode.InternalServerError;
                }
#if DEBUG
                finally
                {
                    outcomeResponse.HttpRequest = request.ToFormattedRequestString();
                    if (response != null)
                    {
                        outcomeResponse.HttpResponse = response.ToFormattedResponseString(null);
                    }
                }
#endif
                return outcomeResponse;

            }
            catch (Exception)
            {
                return new OutcomeResponse { StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        private static async Task<OutcomeResponse<T>> GetOutcome<T>(string serviceUrl, string consumerKey,
            string consumerSecret, string contentType) where T : class
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, serviceUrl);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                SignRequest(request, null, consumerKey, consumerSecret);
                
                var outcomeResponse = new OutcomeResponse<T>();
                HttpResponseMessage response = null;
                try
                {
                    response = await request.GetResponseAsync(allowAutoRedirect: true);
                    outcomeResponse.StatusCode = response.StatusCode;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        outcomeResponse.Outcome = response.DeserializeObject<T>();
                    }
                }
                catch (HttpRequestException)
                {
                    outcomeResponse.StatusCode = response.StatusCode;
                }
                catch (Exception)
                {
                    outcomeResponse.StatusCode = HttpStatusCode.InternalServerError;
                }
                finally
                {
#if DEBUG
                    outcomeResponse.HttpRequest = request.ToFormattedRequestString();
                    if (response != null)
                    {
                        outcomeResponse.HttpResponse = response.ToFormattedResponseString(
                            outcomeResponse.Outcome == null
                            ? null
                            : outcomeResponse.Outcome.ToJsonString());
                    }
#endif
                }
                return outcomeResponse;

            }
            catch (Exception)
            {
                return new OutcomeResponse<T> { StatusCode = HttpStatusCode.InternalServerError };
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

        private static async Task<OutcomeResponse<T>> PostOutcome<T>(T outcome, string serviceUrl,
            string consumerKey, string consumerSecret, string contentType) where T : class
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, serviceUrl);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                var body = Encoding.UTF8.GetBytes(outcome.ToJsonLdString());
                request.Content = new ByteArrayContent(body);

                SignRequest(request, body, consumerKey, consumerSecret);

                var outcomeResponse = new OutcomeResponse<T>();
                HttpResponseMessage response = null;
                try
                {
                    response = await request.GetResponseAsync();
                    outcomeResponse.StatusCode = response.StatusCode;
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        outcomeResponse.Outcome = response.DeserializeObject<T>();
                    }
                }
                catch (HttpRequestException)
                {
                    outcomeResponse.StatusCode = response.StatusCode;
                }
                catch (Exception)
                {
                    outcomeResponse.StatusCode = HttpStatusCode.InternalServerError;
                }
                finally
                {
#if DEBUG
                    outcomeResponse.HttpRequest = request.ToFormattedRequestString();
                    if (response != null)
                    {
                        outcomeResponse.HttpResponse = response.ToFormattedResponseString(
                            outcomeResponse.Outcome == null
                            ? null
                            : outcomeResponse.Outcome.ToJsonString());
                    }
#endif
                }
                return outcomeResponse;

            }
            catch (Exception)
            {
                return new OutcomeResponse<T>
                {
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }

        private static async Task<OutcomeResponse> PutOutcome<T>(T outcome, string serviceUrl, string consumerKey,
            string consumerSecret, string contentType)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Put, serviceUrl);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                var body = Encoding.UTF8.GetBytes(outcome.ToJsonLdString());
                request.Content = new ByteArrayContent(body);

                SignRequest(request, body, consumerKey, consumerSecret);

                var outcomeResponse = new OutcomeResponse();
                HttpResponseMessage response = null;
                try
                {
                    response = await request.GetResponseAsync();
                    outcomeResponse.StatusCode = response.StatusCode;
                }
                catch (HttpRequestException)
                {
                    outcomeResponse.StatusCode = response.StatusCode;
                }
                catch (Exception)
                {
                    outcomeResponse.StatusCode = HttpStatusCode.InternalServerError;
                }
                finally
                {
#if DEBUG
                    outcomeResponse.HttpRequest = request.ToFormattedRequestString();
                    if (response != null)
                    {
                        outcomeResponse.HttpResponse = response.ToFormattedResponseString(null);
                    }
#endif
                }
                return outcomeResponse;
            }
            catch (Exception)
            {
                return new OutcomeResponse { StatusCode = HttpStatusCode.InternalServerError };
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
            request.Content.Headers.Add(OAuthConstants.AuthorizationHeader, authorization.ToString(0, authorization.Length - 1));
        }

        #endregion
    }
}
