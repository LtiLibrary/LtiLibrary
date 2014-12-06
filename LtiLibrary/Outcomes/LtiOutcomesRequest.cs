using LtiLibrary.Common;
using LtiLibrary.Extensions;
using LtiLibrary.OAuth;
using System;
using System.Net.Http;
using System.Security.Cryptography;

namespace LtiLibrary.Outcomes
{
    public class LtiOutcomesRequest : OAuthRequest
    {
        public void ParseRequest(HttpRequestMessage request)
        {
            HttpMethod = request.Method.Method;
            Url = request.RequestUri;

            // All of the OAuth parameters in an LTI Outcomes request are in the
            // Authorization header
            var authorizationHeader = request.Headers.Authorization;
            if (authorizationHeader == null)
            {
                throw new LtiException("No Authorization header");
            }
            if (!authorizationHeader.Scheme.Equals(OAuthConstants.AuthScheme))
            {
                throw new LtiException("Invalid Authorization scheme");
            }

            // Parse the Authorization parameter
            Parameters.Add(authorizationHeader.ParseOAuthAuthorizationHeader());

            // Compare the body hash to make sure the content was not
            // tampered with
            using (var sha1 = new SHA1CryptoServiceProvider())
            using (var task = request.Content.ReadAsStreamAsync())
            {
                task.Wait(3000);
                if (!task.IsCompleted)
                {
                    throw new LtiException("Timeout reading content of request");
                }
                task.Result.Position = 0;
                var hash = sha1.ComputeHash(task.Result);
                var hash64 = Convert.ToBase64String(hash);
                // Reset the position to 0 as a courtesy to others
                task.Result.Position = 0;

                if (!hash64.Equals(BodyHash))
                {
                    throw new LtiException("OAuth body hash does not match");
                }
            }
        }
    }
}
