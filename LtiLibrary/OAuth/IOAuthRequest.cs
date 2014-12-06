using System;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;

namespace LtiLibrary.OAuth
{
    public interface IOAuthRequest
    {
        string BodyHash { get; set; }
        string CallBack { get; set; }
        string ConsumerKey { get; set; }
        string CustomParameters { get; set; }
        string HttpMethod { get; set; }
        string Nonce { get; set; }
        [NotMapped]
        NameValueCollection Parameters { get; }
        [NotMapped]
        string Signature { get; set; }
        string SignatureMethod { get; set; }
        Int64 Timestamp { get; set; }
        [NotMapped]
        DateTime TimestampAsDateTime { get; set; }
        [NotMapped]
        Uri Url { get; set; }
        string Version { get; set; }
    }
}
