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
        /// <param name="request">The request object that will be sent</param>
        /// <param name="consumerKey">The OAuth consumer key.</param>
        /// <param name="consumerSecret">The OAuth consumer secret.</param>
        /// <param name="signatureMethod">The SignatureMethod used to sign the request.</param>
        /// <returns></returns>
        public static async Task SignRequest(HttpRequestMessage request, string consumerKey, string consumerSecret, SignatureMethod signatureMethod)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            
            var ltiRequest = new LtiRequest(LtiConstants.LtiOauthBodyHashMessageType)
            {
                ConsumerKey = consumerKey,
                HttpMethod = request.Method.Method,
                Url = request.RequestUri
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
                var hash = sha.ComputeHash(await (request.Content ?? new StringContent(string.Empty)).ReadAsByteArrayAsync());
                authorizationHeader = ltiRequest.GenerateAuthorizationHeader(hash, consumerSecret);
            }

            // Attach the header to the client request
            request.Headers.Authorization = authorizationHeader;
        }
    }
}
