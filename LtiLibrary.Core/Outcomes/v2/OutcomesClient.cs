using System;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Extensions;
using LtiLibrary.Core.OAuth;

namespace LtiLibrary.Core.Outcomes.v2
{
    public static class OutcomesClient
    {
        public static async Task<OutcomeResponse> DeleteLineItem(string serviceUrl, string consumerKey,
            string consumerSecret)
        {

            return await DeleteOutcome(serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemMediaType);
        }

        public static async Task<OutcomeResponse> DeleteLineItemResult(string serviceUrl, string consumerKey,
            string consumerSecret)
        {

            return await DeleteOutcome(serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemResultsMediaType);
        }

        public static async Task<OutcomeResponse> DeleteLisResult(string serviceUrl, string consumerKey,
            string consumerSecret)
        {

            return await DeleteOutcome(serviceUrl, consumerKey, consumerSecret);
        }

        public static async Task<OutcomeResponse<LineItem>> GetLineItem(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await GetOutcome<LineItem>(serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemMediaType);
        }

        public static async Task<OutcomeResponse<LineItemContainerPage>> GetLineItemPage(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await GetOutcome<LineItemContainerPage>(serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemContainerMediaType);
        }

        public static async Task<OutcomeResponse<LineItem>> GetLineItemResults(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await GetOutcome<LineItem>(serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemResultsMediaType);
        }

        public static async Task<OutcomeResponse<ResultContainer>> GetLisResultPage(string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await GetOutcome<ResultContainer>(serviceUrl, consumerKey, consumerSecret, LtiConstants.LisResultContainerMediaType);
        }

        public static async Task<OutcomeResponse<LineItem>> PostLineItem(LineItem lineItem, string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await PostOutcome(lineItem, serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemMediaType);
        }

        public static async Task<OutcomeResponse<LisResult>> PostLisResult(LisResult lisResult, string serviceUrl, string consumerKey,
            string consumerSecret)
        {
            return await PostOutcome(lisResult, serviceUrl, consumerKey, consumerSecret, LtiConstants.LisResultMediaType);
        }

        public static async Task<OutcomeResponse> PutLineItem(LineItem lineItem, string serviceUrl, string consumerKey, string consumerSecret)
        {
            return await PutOutcome(lineItem, serviceUrl, consumerKey, consumerSecret, LtiConstants.LineItemMediaType);
        }

        public static async Task<OutcomeResponse> PutLisResult(LisResult lisResult, string serviceUrl, string consumerKey, string consumerSecret)
        {
            return await PutOutcome(lisResult, serviceUrl, consumerKey, consumerSecret, LtiConstants.LisResultMediaType);
        }

        #region Private Methods

        private static async Task<OutcomeResponse> DeleteOutcome(string serviceUrl, string consumerKey,
            string consumerSecret, string contentType = null)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
                request.Method = "DELETE";
                if (!string.IsNullOrWhiteSpace(contentType))
                {
                    request.ContentType = contentType;
                }

                SignRequest(request, null, consumerKey, consumerSecret);

                return await Task.Factory.StartNew(() =>
                {
                    var outcomeResponse = new OutcomeResponse();
                    HttpWebResponse response = null;
                    try
                    {
                        response = (HttpWebResponse)request.GetResponse();
                        outcomeResponse.StatusCode = response.StatusCode;
                    }
                    catch (WebException ex)
                    {
                        response = (HttpWebResponse)ex.Response;
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
                });
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
                var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
                request.Method = "GET";
                request.Accept = contentType;
                request.AllowAutoRedirect = true;

                SignRequest(request, null, consumerKey, consumerSecret);

                return await Task.Factory.StartNew(() =>
                {
                    var outcomeResponse = new OutcomeResponse<T>();
                    HttpWebResponse response = null;
                    try
                    {
                        response = (HttpWebResponse)request.GetResponse();
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            outcomeResponse.Outcome = response.DeserializeObject<T>();
                        }
                    }
                    catch (WebException ex)
                    {
                        response = (HttpWebResponse)ex.Response;
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
                });
            }
            catch (Exception)
            {
                return new OutcomeResponse<T> { StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        private static async Task<OutcomeResponse<T>> PostOutcome<T>(T outcome, string serviceUrl,
            string consumerKey, string consumerSecret, string contentType) where T : class
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
                request.Method = "POST";
                request.ContentType = contentType;

                var body = Encoding.UTF8.GetBytes(outcome.ToJsonLdString());
                using (var stream = request.GetRequestStream())
                {
                    await stream.WriteAsync(body, 0, body.Length);
                }

                SignRequest(request, body, consumerKey, consumerSecret);

                return await Task.Factory.StartNew(() =>
                {
                    var outcomeResponse = new OutcomeResponse<T>();
                    HttpWebResponse response = null;
                    try
                    {
                        response = (HttpWebResponse)request.GetResponse();
                        outcomeResponse.StatusCode = response.StatusCode;
                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            outcomeResponse.Outcome = response.DeserializeObject<T>();
                        }
                    }
                    catch (WebException ex)
                    {
                        response = (HttpWebResponse)ex.Response;
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
                });
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
                var request = (HttpWebRequest)WebRequest.Create(serviceUrl);
                request.Method = "PUT";
                request.ContentType = contentType;

                var body = Encoding.UTF8.GetBytes(outcome.ToJsonLdString());
                using (var stream = request.GetRequestStream())
                {
                    await stream.WriteAsync(body, 0, body.Length);
                }

                SignRequest(request, body, consumerKey, consumerSecret);

                return await Task.Factory.StartNew(() =>
                {
                    var outcomeResponse = new OutcomeResponse();
                    HttpWebResponse response = null;
                    try
                    {
                        response = (HttpWebResponse)request.GetResponse();
                        outcomeResponse.StatusCode = response.StatusCode;
                    }
                    catch (WebException ex)
                    {
                        response = (HttpWebResponse)ex.Response;
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
                });
            }
            catch (Exception)
            {
                return new OutcomeResponse { StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        private static void SignRequest(WebRequest request, byte[] body, string consumerKey, string consumerSecret)
        {
            if (body == null)
                body = new byte[0];
            if (body.Length > 0 && request.ContentLength != body.Length)
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
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var hash = sha1.ComputeHash(body);
                var hash64 = Convert.ToBase64String(hash);
                parameters.AddParameter(OAuthConstants.BodyHashParameter, hash64);
            }

            // Calculate the signature
            var signature = OAuthUtility.GenerateSignature(request.Method, request.RequestUri, parameters,
                consumerSecret);
            parameters.AddParameter(OAuthConstants.SignatureParameter, signature);

            // Build the Authorization header
            var authorization = new StringBuilder(OAuthConstants.AuthScheme).Append(" ");
            foreach (var key in parameters.AllKeys)
            {
                authorization.AppendFormat("{0}=\"{1}\",", key, WebUtility.UrlEncode(parameters[key]));
            }
            request.Headers["Authorization"] = authorization.ToString(0, authorization.Length - 1);
        }

        #endregion
    }
}
