using System;

namespace LtiLibrary.NetCore.OAuth
{
    /// <summary>
    /// OAuth 1.0 constants
    /// </summary>
    public static class OAuthConstants
    {
        /// <summary>
        /// Authorization header name.
        /// </summary>
        public const string AuthorizationHeader = "Authorization";

        /// <summary>
        /// OAuth scheme name.
        /// </summary>
        public const string AuthScheme = "OAuth";

        /// <summary>
        /// oauth_body_hash parameter name.
        /// </summary>
        public const string BodyHashParameter = "oauth_body_hash";

        /// <summary>
        /// oauth_callback parameter name.
        /// </summary>
        public const string CallbackParameter = "oauth_callback";

        /// <summary>
        /// oauth_callback default value (about:blank).
        /// </summary>
        public const string CallbackDefault = "about:blank";

        /// <summary>
        /// oauth_consumer_key parameter name.
        /// </summary>
        public const string ConsumerKeyParameter = "oauth_consumer_key";

        /// <summary>
        /// OAuth epoch (1/1/1970 00:00:00).
        /// </summary>
        public static DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// oauth_nonce parameter name.
        /// </summary>
        public const string NonceParameter = "oauth_nonce";

        /// <summary>
        /// realm parameter name.
        /// </summary>
        public const string RealmParameter = "realm";

        /// <summary>
        /// oauth_signature parameter name.
        /// </summary>
        public const string SignatureParameter = "oauth_signature"; 

        /// <summary>
        /// oauth_signature_method parameter name.
        /// </summary>
        public const string SignatureMethodParameter = "oauth_signature_method";

        /// <summary>
        /// HMAC-SHA1 signature method.
        /// </summary>
        public const string SignatureMethodHmacSha1 = "HMAC-SHA1";

        /// <summary>
        /// oauth_timestamp parameter name.
        /// </summary>
        public const string TimestampParameter = "oauth_timestamp";

        /// <summary>
        /// oauth_token parameter name.
        /// </summary>
        public const string TokenParameter = "oauth_token";

        /// <summary>
        /// oauth_version parameter name.
        /// </summary>
        public const string VersionParameter = "oauth_version";

        /// <summary>
        /// Version 1.0.
        /// </summary>
        public const string Version10 = "1.0";

        /// <summary>
        /// Array of OAuth parameters.
        /// </summary>
        public static readonly string[] OAuthParameters =
            {
                BodyHashParameter,
                CallbackParameter,
                ConsumerKeyParameter,
                NonceParameter,
                RealmParameter,
                SignatureParameter,
                SignatureMethodParameter,
                TimestampParameter,
                TokenParameter,
                VersionParameter
            };
    }
}
