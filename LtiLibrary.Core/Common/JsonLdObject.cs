using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LtiLibrary.Core.Common
{
    [JsonConverter(typeof(JsonLdObjectConverter))]
    public class JsonLdObject : IJsonLdObject
    {
        public JsonLdObject()
        {
            Terms = new Dictionary<string, string>();
        }

        /// <summary>
        /// Optional @id of an external context to define the short-hand names that are used throughout a JSON-LD 
        /// document to express specific identifiers in a compact manner.
        /// </summary>
        [JsonIgnore]
        public Uri ExternalContextId { get; set; }

        /// <summary>
        /// Optional terms to define the short-hand names that are used throughout a JSON-LD 
        /// document to express specific identifiers in a compact manner.
        /// </summary>
        [JsonIgnore]
        public IDictionary<string, string> Terms { get; }

        /// <summary>
        /// JSON-LD context created by JsonLdObjectConverter when serialized.
        /// </summary>
        [JsonProperty("@context", Order = -4)]
        public object Context { get; internal set; }

        /// <summary>
        /// Optional URI to uniquely identify things that are being described in the document with IRIs 
        /// or blank node identifiers.
        /// </summary>
        [JsonProperty("@id", Order = -3)]
        public Uri Id { get; set; }

        /// <summary>
        /// Optional data type of a node or typed value.
        /// </summary>
        [JsonProperty("@type", Order = -2)]
        public string Type { get; set; }
    }
}
