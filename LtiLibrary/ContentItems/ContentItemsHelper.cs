using LtiLibrary.Common;
using LtiLibrary.Extensions;
using LtiLibrary.Lti1;
using LtiLibrary.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace LtiLibrary.ContentItems
{
    public static class ContentItemsHelper
    {
        /// <summary>
        /// Create an LtiRequestViewModel that contains a ContentItemPlacementResponse.
        /// </summary>
        /// <param name="url">The content_item_return_url from the content-item message.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to sign the request.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to sign the request.</param>
        /// <param name="contentItems">The ContentItemPlacementResponse to send.</param>
        /// <param name="data">The data received in the original content-item message.</param>
        /// <returns>The LtiRequestViewModel which contains a signed version of the response.</returns>
        public static LtiRequestViewModel CreateContentItemSelectionResponseViewModel(
            string url, string consumerKey, string consumerSecret,
            ContentItemPlacementResponse contentItems, string data)
        {
            return CreateContentItemSelectionResponseViewModel(url, consumerKey, consumerSecret, contentItems, data, null, null, null, null);
        }

        /// <summary>
        /// Create an LtiRequestViewModel that contains a ContentItemPlacementResponse.
        /// </summary>
        /// <param name="url">The content_item_return_url from the content-item message.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to sign the request.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to sign the request.</param>
        /// <param name="contentItems">The ContentItemPlacementResponse to send.</param>
        /// <param name="data">The data received in the original content-item message.</param>
        /// <param name="ltiErrorLog">Optional plain text error message to be logged by the Tool Consumer.</param>
        /// <param name="ltiErrorMsg">Optional plain text error message to be displayed by the Tool Consumer.</param>
        /// <param name="ltiLog">Optional plain text message to be logged by the Tool Consumer.</param>
        /// <param name="ltiMsg">Optional plain text message to be displayed by the Tool Consumer.</param>
        /// <returns>The LtiRequestViewModel which contains a signed version of the response.</returns>
        public static LtiRequestViewModel CreateContentItemSelectionResponseViewModel(
            string url, string consumerKey, string consumerSecret,
            ContentItemPlacementResponse contentItems, string data,
            string ltiErrorLog, string ltiErrorMsg, string ltiLog, string ltiMsg)
        {
            var ltiRequest = (IContentItemSelectionResponse)new LtiRequest(LtiConstants.ContentItemSelectionResponseLtiMessageType)
            {
                Url = new Uri(url),
                ConsumerKey = consumerKey,
                ContentItems = JsonConvert.SerializeObject(contentItems, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                Data = data,
                LtiErrorLog = ltiErrorLog,
                LtiErrorMsg = ltiErrorMsg,
                LtiLog = ltiLog,
                LtiMsg = ltiMsg
            };

            return ltiRequest.GetLtiRequestViewModel(consumerSecret);
        }

        /// <summary>
        /// Create an HttpWebRequest for a content-item service message. This is experimental.
        /// </summary>
        /// <param name="url">The content-item service URL.</param>
        /// <param name="consumerKey">The OAuth consumer key to use to sign the request.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to sign the request.</param>
        /// <param name="response">The ContentItemsServiceResponse to POST.</param>
        /// <returns>The HttpWebRequest that will POST the message.</returns>
        private static HttpWebRequest CreateContentItemsRequest(
            string url, string consumerKey, string consumerSecret, 
            ContentItemsServiceResponse response)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = LtiConstants.ContentItemsMediaType;

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
                var json = JsonConvert.SerializeObject(response, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var content = Encoding.Unicode.GetBytes(json);
                webRequest.ContentLength = content.Length;
                using (var stream = webRequest.GetRequestStream())
                {
                    stream.Write(content, 0, content.Length);
                }

                var hash = sha1.ComputeHash(content);
                var hash64 = Convert.ToBase64String(hash);
                parameters.AddParameter(OAuthConstants.BodyHashParameter, hash64);
            }

            // Calculate the signature
            var signature = OAuthUtility.GenerateSignature(webRequest.Method, webRequest.RequestUri, parameters,
                consumerSecret);
            parameters.AddParameter(OAuthConstants.SignatureParameter, signature);

            // Build the Authorization header
            var authorization = new StringBuilder(OAuthConstants.AuthScheme).Append(" ");
            foreach (var key in parameters.AllKeys)
            {
                authorization.AppendFormat("{0}=\"{1}\",", key, HttpUtility.UrlEncode(parameters[key]));
            }
            webRequest.Headers["Authorization"] = authorization.ToString(0, authorization.Length - 1);

            return webRequest;
        }

        /// <summary>
        /// Send content-item response to Tool Consumer content-item service. This is experimental.
        /// </summary>
        /// <param name="url">The content-item service URL (from content-item message).</param>
        /// <param name="consumerKey">The OAuth consumer key to use to sign the request.</param>
        /// <param name="consumerSecret">The OAuth consumer secret to use to sign the request.</param>
        /// <param name="contentItems">The ContentItemPlacementResponse to POST.</param>
        /// <param name="data">The data received in the original content-item message.</param>
        /// <returns>Return True if the POST request is successful.</returns>
        public static bool PostContentItems(
            string url, string consumerKey, string consumerSecret, 
            ContentItemPlacementResponse contentItems, string data)
        {
            var serviceResponse = new ContentItemsServiceResponse
            {
                ContentItems = contentItems,
                Data = data
            };

            try
            {
                var webRequest = CreateContentItemsRequest(url, consumerKey, consumerSecret, serviceResponse);
                using (var webResponse = webRequest.GetResponse() as HttpWebResponse)
                {
                    return webResponse != null && webResponse.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
