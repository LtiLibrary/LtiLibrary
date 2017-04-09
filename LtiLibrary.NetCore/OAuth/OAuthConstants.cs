using System;

namespace LtiLibrary.NetCore.OAuth
{
    public static class OAuthConstants
    {
        public const string AuthorizationHeader = "Authorization";
        public const string AuthScheme = "OAuth";
        public const string BodyHashParameter = "oauth_body_hash";
        public const string CallbackParameter = "oauth_callback";
        public const string CallbackDefault = "about:blank";
        public const string ConsumerKeyParameter = "oauth_consumer_key";
        public static DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public const string NonceParameter = "oauth_nonce";
        public const string RealmParameter = "realm";
        public const string SignatureParameter = "oauth_signature"; 
        public const string SignatureMethodParameter = "oauth_signature_method";
        public const string SignatureMethodHmacSha1 = "HMAC-SHA1";
        public const string TimestampParameter = "oauth_timestamp";
        public const string TokenParameter = "oauth_token";
        public const string VersionParameter = "oauth_version";
        public const string Version10 = "1.0";

        // OAuth 1.0 parameters
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
