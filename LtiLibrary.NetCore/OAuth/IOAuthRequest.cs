using System;
using System.Collections.Specialized;

namespace LtiLibrary.NetCore.OAuth
{
    internal interface IOAuthRequest
    {
        string BodyHash { get; set; }
        string CallBack { get; set; }
        string ConsumerKey { get; set; }
        string CustomParameters { get; set; }
        string HttpMethod { get; set; }
        string Nonce { get; set; }
        NameValueCollection Parameters { get; }
        string Signature { get; set; }
        string SignatureMethod { get; set; }
        Int64 Timestamp { get; set; }
        DateTime TimestampAsDateTime { get; set; }
        Uri Url { get; set; }
        string Version { get; set; }
    }
}
