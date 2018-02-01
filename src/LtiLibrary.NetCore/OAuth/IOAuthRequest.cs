using System;

namespace LtiLibrary.NetCore.OAuth
{
    /// <summary>
    /// Interface for an OAuth 1.0a request.
    /// RFC5849: https://tools.ietf.org/html/rfc5849
    /// OAuth Core 1.0: https://oauth.net/core/1.0/
    /// OAuth Core 1.0a: https://oauth.net/core/1.0a/
    /// </summary>
    public interface IOAuthRequest
    {
        /// <summary>
        /// An absolute URI back to which the server will redirect the resource owner when the Resource Owner Authorization step(Section 2.2) is completed.
        /// </summary>
        string CallBack { get; set; }

        /// <summary>
        /// The identifier portion of the client credentials (equivalent to a username).
        /// </summary>
        string ConsumerKey { get; set; }

        /// <summary>
        /// The nonce value as defined in Section 3.3.
        /// </summary>
        string Nonce { get; set; }

        /// <summary>
        /// The name of the signature method used by the client to sign the request, as defined in Section 3.4.
        /// </summary>
        string SignatureMethod { get; set; }

        /// <summary>
        /// The timestamp value as defined in Section 3.3.
        /// </summary>
        long Timestamp { get; set; }

        /// <summary>
        /// OPTIONAL.  If present, MUST be set to "1.0".
        /// </summary>
        string Version { get; set; }

        #region Non-OAuth Properties

        /// <summary>
        /// Hash of the request content.
        /// </summary>
        string BodyHash { get; set; }

        /// <summary>
        /// HTTP Method of the request.
        /// </summary>
        string HttpMethod { get; set; }

        /// <summary>
        /// Request signature.
        /// </summary>
        string Signature { get; set; }

        /// <summary>
        /// Timestamp as a DateTime.
        /// </summary>
        DateTime TimestampAsDateTime { get; set; }

        /// <summary>
        /// URI of the request.
        /// </summary>
        Uri Url { get; set; }

        #endregion
    }
}
