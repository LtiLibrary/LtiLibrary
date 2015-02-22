using Newtonsoft.Json;

namespace LtiLibrary.Core.Common
{
    public class JsonLdObject
    {
        public JsonLdObject(string type)
        {
            Type = type;
        }

        /// <summary>
        /// Optional terms to define the short-hand names that are used throughout a JSON-LD 
        /// document to express specific identifiers in a compact manner.
        /// </summary>
        [JsonProperty("@context")]
        public object LdContext { get; set; }

        /// <summary>
        /// Optional URI to uniquely identify things that are being described in the document with IRIs 
        /// or blank node identifiers.
        /// </summary>
        [JsonProperty("@id")]
        public string Id { get; set; }

        /// <summary>
        /// Optional data type of a node or typed value.
        /// </summary>
        [JsonProperty("@type")]
        public string Type { get; private set; }
    }
}
