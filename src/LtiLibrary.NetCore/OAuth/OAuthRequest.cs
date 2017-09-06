﻿using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;

namespace LtiLibrary.NetCore.OAuth
{
    /// <summary>
    /// Represents an OAuth 1.0a HTTP request.
    /// </summary>
    [DataContract]
    public class OAuthRequest : IOAuthRequest
    {
        /// These OAuth parameters are required on all LtiRequests
        protected static readonly string[] RequiredOauthParameters =
        {
            OAuthConstants.ConsumerKeyParameter,
            OAuthConstants.NonceParameter,
            OAuthConstants.SignatureMethodParameter,
            OAuthConstants.TimestampParameter,
            OAuthConstants.VersionParameter
        };

        /// These OAuth parameters are required in OAuth Authorization Headers
        protected static readonly string[] RequiredOauthAuthorizationHeaderParameters =
        {
            OAuthConstants.BodyHashParameter,
            OAuthConstants.ConsumerKeyParameter,
            OAuthConstants.NonceParameter,
            OAuthConstants.SignatureParameter,
            OAuthConstants.SignatureMethodParameter,
            OAuthConstants.TimestampParameter,
            OAuthConstants.VersionParameter
        };

        /// <summary>
        /// Initialize a new instance of the OAuthRequest class.
        /// </summary>
        public OAuthRequest()
        {
            Parameters = new NameValueCollection();
        }

        /// <summary>
        /// The OAuth body hash from the Authorization header.
        /// </summary>
        /// <remarks>
        /// This hash is calculated by the sender.
        /// </remarks>
        [DataMember(Name = OAuthConstants.BodyHashParameter)]
        public string BodyHash
        {
            get
            {
                return Parameters[OAuthConstants.BodyHashParameter];
            }
            set
            {
                Parameters[OAuthConstants.BodyHashParameter] = value;
            }
        }

        /// <summary>
        /// The OAuth body hash calculated by LtiLibrary.AspNetCore.Common.AddBodyHashHeaderAttribute.
        /// </summary>
        /// <remarks>
        /// This hash is calculated by the receiver.
        /// </remarks>
        public string BodyHashReceived { get; set; }

        /// <summary>
        /// Not used in OAuth 1.0a.
        /// </summary>
        [DataMember(Name = OAuthConstants.CallbackParameter)]
        public string CallBack
        {
            get
            {
                return Parameters[OAuthConstants.CallbackParameter];
            }
            set
            {
                Parameters[OAuthConstants.CallbackParameter] = value;
            }
        }

        /// <summary>
        /// OAuth consumer key
        /// </summary>
        [DataMember(Name = OAuthConstants.ConsumerKeyParameter)]
        public string ConsumerKey
        {
            get
            {
                return Parameters[OAuthConstants.ConsumerKeyParameter];
            }
            set
            {
                Parameters[OAuthConstants.ConsumerKeyParameter] = value;
            }
        }

        /// <summary>
        /// The custom_ and ext_ parameters in Querystring format suitable for saving in the database.
        /// </summary>
        [DataMember]
        public string CustomParameters
        {
            get
            {
                var customParameters = new UrlEncodingParser("");
                foreach (var key in Parameters.AllKeys)
                {
                    if (key.StartsWith("custom_") || key.StartsWith("ext_"))
                    {
                        customParameters.Add(key, Parameters[key]);
                    }
                }

                return customParameters.Count == 0 ? null : customParameters.ToString();
            }
            set
            {
                var customParameters = new UrlEncodingParser(value);
                foreach (var key in customParameters.AllKeys)
                {
                    if (key.StartsWith("custom_") || key.StartsWith("_ext"))
                    {
                        Parameters[key] = customParameters[key];
                    }
                }
            }
        }

        /// <summary>
        /// The HTTP Method of the request
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// OAuth nonce
        /// </summary>
        [DataMember(Name = OAuthConstants.NonceParameter)]
        public string Nonce
        {
            get
            {
                return Parameters[OAuthConstants.NonceParameter];
            }
            set
            {
                Parameters[OAuthConstants.NonceParameter] = value;
            }
        }

        /// <summary>
        /// All the OAuth parameters in the request
        /// </summary>
        public NameValueCollection Parameters { get; }

        /// <summary>
        /// OAuth signature
        /// </summary>
        [DataMember(Name = OAuthConstants.SignatureParameter)]
        public string Signature
        {
            get
            {
                return Parameters[OAuthConstants.SignatureParameter];
            }
            set
            {
                Parameters[OAuthConstants.SignatureParameter] = value;
            }
        }

        /// <summary>
        /// The OAuth signature method.
        /// </summary>
        [DataMember(Name = OAuthConstants.SignatureMethodParameter)]
        public string SignatureMethod
        {
            get
            {
                return Parameters[OAuthConstants.SignatureMethodParameter];
            }
            set
            {
                Parameters[OAuthConstants.SignatureMethodParameter] = value;
            }
        }

        /// <summary>
        /// OAuth timestamp (number of seconds since 1/1/1970)
        /// </summary>
        [DataMember(Name = OAuthConstants.TimestampParameter)]
        public long Timestamp
        {
            get
            {
                return Convert.ToInt64(Parameters[OAuthConstants.TimestampParameter]);
            }
            set
            {
                Parameters[OAuthConstants.TimestampParameter] = Convert.ToString(value);
            }
        }

        /// <summary>
        /// Convenience property to get and set the OAuth Timestamp using DateTime
        /// </summary>
        public DateTime TimestampAsDateTime
        {
            get
            {
                return OAuthConstants.Epoch.AddSeconds(Timestamp);
            }
            set
            {
                Timestamp = Convert.ToInt64((value - OAuthConstants.Epoch).TotalSeconds);                
            }
        }

        /// <summary>
        /// The resource URL.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// The OAuth version.
        /// </summary>
        [DataMember(Name = OAuthConstants.VersionParameter)]
        public string Version
        {
            get
            {
                return Parameters[OAuthConstants.VersionParameter];
            }
            set
            {
                Parameters[OAuthConstants.VersionParameter] = value;
            }
        }

        /// <summary>
        /// Create an OAuth <see cref="AuthenticationHeaderValue"/> for this OAuthRequest.
        /// </summary>
        /// <returns>The computed AuthorizationHeader</returns>
        internal AuthenticationHeaderValue GenerateAuthorizationHeader(byte[] bodyHash, string consumerSecret)
        {
            BodyHash = Convert.ToBase64String(bodyHash);
            Signature = GenerateSignature(consumerSecret);

            // Build the Authorization header
            var authorizationHeader = new StringBuilder(OAuthConstants.AuthScheme).Append(" ");
            foreach (var key in RequiredOauthAuthorizationHeaderParameters)
            {
                authorizationHeader.AppendFormat("{0}=\"{1}\",", key, WebUtility.UrlEncode(Parameters[key]));
            }
            return AuthenticationHeaderValue.Parse(authorizationHeader.ToString(0, authorizationHeader.Length - 1));
        }

        /// <summary>
        /// Calculate the OAuth Signature for this request using the parameters in the request.
        /// </summary>
        /// <param name="consumerSecret">The OAuth Consumer Secret to use.</param>
        /// <returns>The calculated OAuth Signature.</returns>
        /// <remarks>This method works whether the parameters are based on form fields or OAuth Authorization header.</remarks>
        public string GenerateSignature(string consumerSecret)
        {
            // If there is no BodyHashReceived, calculate signature based on form parameters
            if (string.IsNullOrEmpty(BodyHashReceived) && string.IsNullOrEmpty(BodyHash))
            {
                return GenerateSignature(HttpMethod, Url, Parameters, consumerSecret);
            }

            // Otherwise calculate the signature using the body hash instead of form parameters
            var parameters = new NameValueCollection();
            foreach (var key in RequiredOauthAuthorizationHeaderParameters)
            {
                parameters[key] = Parameters[key];
            }
            if (!string.IsNullOrEmpty(BodyHashReceived))
            {
                parameters[OAuthConstants.BodyHashParameter] = BodyHashReceived;
            }
            return GenerateSignature(HttpMethod, Url, parameters, consumerSecret);
        }

        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="parameters"></param>
        /// <returns>The signature base</returns>
        private static string GenerateSignatureBase(string httpMethod, Uri url, NameValueCollection parameters)
        {
            // https://tools.ietf.org/html/rfc5849#section-3.4.1.1
            var signatureBase = new StringBuilder();
            signatureBase.Append(httpMethod.ToRfc3986EncodedString().ToUpperInvariant()).Append('&');

            // https://tools.ietf.org/html/rfc5849#section-3.4.1.2
            // Exclude the query (query parameters in parameters collection) from the URI
            var normalizedUrl = $"{url.Scheme.ToLowerInvariant()}://{url.Host.ToLowerInvariant()}";
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;
            signatureBase.Append(normalizedUrl.ToRfc3986EncodedString()).Append('&');

            // Construct the signature string
            signatureBase.Append(parameters.ToNormalizedString().ToRfc3986EncodedString());

            return signatureBase.ToString();
        }

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>
        /// <param name="httpMethod">The http method used</param>
        /// <param name="url">The full url to be signed</param>
        /// <param name="parametersIn">The collection of parameters to sign</param>
        /// <param name="consumerSecret">The OAuth consumer secret used to generate the signature</param>
        /// <returns>A base64 string of the hash value</returns>
        private static string GenerateSignature(string httpMethod, Uri url, NameValueCollection parametersIn, string consumerSecret)
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
                Key = Encoding.ASCII.GetBytes($"{consumerSecret.ToRfc3986EncodedString()}&")
            };

            var dataBuffer = Encoding.ASCII.GetBytes(signatureBase);
            var hashBytes = hmacsha1.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
