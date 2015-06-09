using System;

namespace LtiLibrary.Core.Common
{
    /// <summary>
    /// URIs are used to describe enumerated field values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class UriAttribute : Attribute
    {
        public UriAttribute(string uri)
        {
            Uri = new Uri(uri);
        }

        public Uri Uri { get; set; }
    }
}
