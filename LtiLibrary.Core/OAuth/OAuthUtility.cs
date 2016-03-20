using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using LtiLibrary.Core.Common;
using LtiLibrary.Core.Extensions;

namespace LtiLibrary.Core.OAuth
{
    public static class OAuthUtility
    {
        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="parameters"></param>
        /// <returns>The signature base</returns>
        public static string GenerateSignatureBase(string httpMethod, Uri url, NameValueCollection parameters)
        {
            // https://tools.ietf.org/html/rfc5849#section-3.4.1.1
            var signatureBase = new StringBuilder();
            signatureBase.Append(httpMethod.ToRfc3986EncodedString().ToUpperInvariant()).Append('&');

            // https://tools.ietf.org/html/rfc5849#section-3.4.1.2
            // Exclude the query (query parameters in parameters collection)
            var normalizedUrl = string.Format("{0}://{1}", url.Scheme.ToLowerInvariant(), url.Host.ToLowerInvariant());
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;
            signatureBase.Append(normalizedUrl.ToRfc3986EncodedString()).Append('&');

            // https://tools.ietf.org/html/rfc5849#section-3.4.1.3
            // Exclude the OAuth signature or realm in the signature base string
            var excludedNames = new List<string> {OAuthConstants.SignatureParameter, OAuthConstants.RealmParameter};

            // Construct the signature string
            signatureBase.Append(parameters.ToNormalizedString(excludedNames).ToRfc3986EncodedString());

            return signatureBase.ToString();
        }

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>
        /// <param name="httpMethod">The http method used</param>
        /// <param name="url">The full url to be signed</param>
        /// <param name="parameters">The collection of parameters to sign</param>
        /// <param name="consumerSecret">The OAuth consumer secret used to generate the signature</param>
        /// <returns>A base64 string of the hash value</returns>
        public static string GenerateSignature(string httpMethod, Uri url, NameValueCollection parametersIn, string consumerSecret)
        {
            // Work with a copy of the parameters so the caller's data is not changed
            var parameters = new NameValueCollection(parametersIn);

            // https://tools.ietf.org/html/rfc5849#section-3.4.1.3.1
            // The query component is parsed into a list of name/value pairs by treating it as an
            // "application/x-www-form-urlencoded" string, separating the names and values and 
            // decoding them as defined by [W3C.REC - html40 - 19980424], Section 17.13.4.
            //
            // Unescape the query so that it is not doubly escaped by UrlEncodingParser.
            var querystring = new UrlEncodingParser(Uri.UnescapeDataString(url.Query));
            parameters.Add(querystring);

            var signatureBase = GenerateSignatureBase(httpMethod, url, parameters);

            // Note that in LTI, the TokenSecret (second part of the key) is blank
            var hmacsha1 = new HMACSHA1
            {
                Key = Encoding.ASCII.GetBytes(string.Format("{0}&", consumerSecret.ToRfc3986EncodedString()))
            };

            var dataBuffer = Encoding.ASCII.GetBytes(signatureBase);
            var hashBytes = hmacsha1.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
