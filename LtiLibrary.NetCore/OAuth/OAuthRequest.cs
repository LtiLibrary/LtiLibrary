using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Extensions;

namespace LtiLibrary.NetCore.OAuth
{
    [DataContract]
    public class OAuthRequest : IOAuthRequest
    {
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
        /// Calculate the OAuth Signature for this request using the parameters in the request.
        /// </summary>
        /// <param name="consumerSecret">The OAuth Consumer Secret to use.</param>
        /// <returns>The calculated OAuth Signature.</returns>
        /// <remarks>This method works whether the parameters are based on form fields or OAuth Authorization header.</remarks>
        public string GenerateSignature(string consumerSecret)
        {
            // If there is no BodyHashReceived, calculate signature based on form parameters
            if (string.IsNullOrEmpty(BodyHashReceived))
            {
                return OAuthUtility.GenerateSignature(HttpMethod, Url, Parameters, consumerSecret);
            }

            // Otherwise calculate the signature using the body hash instead of form parameters
            var parameters = new NameValueCollection();
            parameters.AddParameter(OAuthConstants.ConsumerKeyParameter, ConsumerKey);
            parameters.AddParameter(OAuthConstants.NonceParameter, Nonce);
            parameters.AddParameter(OAuthConstants.SignatureMethodParameter, OAuthConstants.SignatureMethodHmacSha1);
            parameters.AddParameter(OAuthConstants.VersionParameter, OAuthConstants.Version10);
            parameters.AddParameter(OAuthConstants.TimestampParameter, Timestamp);
            parameters.AddParameter(OAuthConstants.BodyHashParameter, BodyHashReceived);
            return OAuthUtility.GenerateSignature(HttpMethod, Url, parameters, consumerSecret);
        }
    }
}
