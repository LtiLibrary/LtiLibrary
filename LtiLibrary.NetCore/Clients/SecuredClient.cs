using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti.v1;

namespace LtiLibrary.NetCore.Clients
{
    /// <summary>
    /// Base class for LTI client helpers that make secured requests.
    /// </summary>
    internal static class SecuredClient
    {
        internal static async Task SignRequest(HttpClient client, HttpMethod method, string serviceUrl,
            HttpContent content, string consumerKey, string consumerSecret)
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

            // Create an Authorization header using the body hash
            using (var sha1 = SHA1.Create())
            {
                var hash = sha1.ComputeHash(await content.ReadAsByteArrayAsync());
                authorizationHeader = ltiRequest.GenerateAuthorizationHeader(hash, consumerSecret);
            }

            // Attach the header to the client request
            client.DefaultRequestHeaders.Authorization = authorizationHeader;
        }
    }
}
