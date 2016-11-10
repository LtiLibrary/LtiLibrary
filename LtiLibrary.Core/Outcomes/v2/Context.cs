using System.Collections.Generic;
using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.Core.Outcomes.v2
{
    /// <summary>
    /// A learning context, such as a Course Section.
    /// </summary>
    public class Context : JsonLdObject
    {
        public Context()
        {
            Type = LtiConstants.ContextType;
        }

        /// <summary>
        /// A unique string provided by the tool consumer to identify the context (as passed in the context_id launch parameter).
        /// </summary>
        [JsonProperty("contextId")]
        public string ContextId { get; set; }

        /// <summary>
        /// Zero or more LineItems that record results for some learning activity within this context.
        /// </summary>
        [JsonProperty("lineItem")]
        public ICollection<LineItem> LineItems { get; set; }

        /// <summary>
        /// Zero or more LISResults that record results for some learning activity within this context.
        /// </summary>
        [JsonProperty("result")]
        public ICollection<LisResult> Results { get; set; }
    }
}
