using System;

namespace LtiLibrary.NetCore.Common
{
    /// <summary>
    /// URIs are used to describe enumerated field values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class UriAttribute : Attribute
    {
        /// <summary>
        /// Initialize a new instance of the UriAttribute class.
        /// </summary>
        /// <param name="uri">The URI associated with the enumerated field value.</param>
        public UriAttribute(string uri)
        {
            Uri = new Uri(uri);
        }

        /// <summary>
        /// Get or Set the URI associated with the enumberated field value.
        /// </summary>
        public Uri Uri { get; set; }
    }
}
