using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti.v1;
using LtiLibrary.NetCore.OAuth;

namespace LtiLibrary.NetCore.Clients
{
    /// <summary>
    /// Base class for LTI client helpers that make secured requests.
    /// </summary>
    public static class SecuredClient
    {
        /// <summary>
        /// Add a signed authorization header to the client request using the signatureMethod.
        /// </summary>
        /// <param name="client">The HttpClient that will make the request.</param>
        /// <param name="method">The HttpMethod of the request.</param>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="content">The Content of the request (used to calculate the body hash).</param>
        /// <param name="consumerKey">The OAuth consumer key.</param>
        /// <param name="consumerSecret">The OAuth consumer secret.</param>
        /// <param name="signatureMethod">The SignatureMethod used to sign the request.</param>
        /// <returns></returns>
        public static async Task SignRequest(HttpClient client, HttpMethod method, string serviceUrl,
            HttpContent content, string consumerKey, string consumerSecret, SignatureMethod signatureMethod)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrEmpty(serviceUrl))
            {
                throw new ArgumentNullException(nameof(serviceUrl));
            }

            var serviceUri = new Uri(serviceUrl, UriKind.RelativeOrAbsolute);
            if (!serviceUri.IsAbsoluteUri)
            {
                if (client.BaseAddress == null)
                {
                    throw new LtiException("If serviceUrl is relative, client.BaseAddress cannot be null.");
                }

                if (!Uri.TryCreate(client.BaseAddress, serviceUrl, out serviceUri))
                {
                    throw new LtiException($"Cannot form a valid URI from {client.BaseAddress} and {serviceUrl}.");
                }
            }
            
            var ltiRequest = new LtiRequest(LtiConstants.LtiOauthBodyHashMessageType)
            {
                ConsumerKey = consumerKey,
                HttpMethod = method.Method,
                Url = serviceUri
            };

            AuthenticationHeaderValue authorizationHeader;

            // Determine hashing algorithm
            HashAlgorithm sha;
            switch(signatureMethod)
            {
                default: //HmacSha1
                    sha = SHA1.Create();
                    break;
                case SignatureMethod.HmacSha256:
                    ltiRequest.SignatureMethod = "HMAC-SHA256";
                    sha = SHA256.Create();
                    break;
                case SignatureMethod.HmacSha384:
                    ltiRequest.SignatureMethod = "HMAC-SHA384";
                    sha = SHA384.Create();
                    break;
                case SignatureMethod.HmacSha512:
                    ltiRequest.SignatureMethod = "HMAC-SHA512";
                    sha = SHA512.Create();
                    break;
            }

            // Create an Authorization header using the body hash
            using (sha)
            {
                var hash = sha.ComputeHash(await content.ReadAsByteArrayAsync());
                authorizationHeader = ltiRequest.GenerateAuthorizationHeader(hash, consumerSecret);
            }

            // Attach the header to the client request
            client.DefaultRequestHeaders.Authorization = authorizationHeader;
        }
    }
}
