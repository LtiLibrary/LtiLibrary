using System;
using System.Collections.Generic;

namespace LtiLibrary.NetCore.Common
{
    /// <summary>
    /// JSON-LD object interface.
    /// </summary>
    internal interface IJsonLdObject
    {
        /// <summary>
        /// JSON-LD @context
        /// </summary>
        object Context { get; set; }

        /// <summary>
        /// Reference to an external JSON-LD @id. Similar to an XML namespace.
        /// </summary>
        Uri ExternalContextId { get; set; }

        /// <summary>
        /// JSON-LD terms. Similar to XML prefixes.
        /// </summary>
        IDictionary<string, string> Terms { get; }

        /// <summary>
        /// JSON-LD @id
        /// </summary>
        Uri Id { get; set; }

        /// <summary>
        /// JDSON-LD @type
        /// </summary>
        string Type { get; set; }
    }
}