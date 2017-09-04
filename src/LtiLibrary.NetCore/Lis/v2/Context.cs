using System.Collections.Generic;
using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lis.v2
{
    /// <summary>
    /// A learning context, such as a Course Section.
    /// </summary>
    public class Context : JsonLdObject
    {
        /// <summary>
        /// Initialize a new instance of the Context class.
        /// </summary>
        public Context()
        {
            Type = LtiConstants.ContextType;
        }

        /// <summary>
        /// Get or set a unique string provided by the tool consumer to identify the context (as passed in the context_id launch parameter).
        /// </summary>
        [JsonProperty("contextId")]
        public string ContextId { get; set; }

        /// <summary>
        /// Zero or more LineItems that record results for some learning activity within this context.
        /// </summary>
        [JsonProperty("lineItem")]
        public ICollection<LineItem> LineItems { get; set; }

        /// <summary>
        /// Get or set the array of membership entities that records the role of some Agent within this Organization.
        /// </summary>
        [JsonProperty("membership")]
        public ICollection<Membership> Membership { get; set; }

        /// <summary>
        /// Zero or more LISResults that record results for some learning activity within this context.
        /// </summary>
        [JsonProperty("result")]
        public ICollection<Result> Results { get; set; }

        /// <summary>
        /// Get or set the organization's assigned name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
