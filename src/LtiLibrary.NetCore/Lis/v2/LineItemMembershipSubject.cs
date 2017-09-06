using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lis.v2
{
    /// <summary>
    /// Represents an IMS LineItemMembershipSubject
    /// </summary>
    public class LineItemMembershipSubject : JsonLdObject
    {
        /// <summary>
        /// Get or set the contextId of the LineItemMembershipSubject.
        /// </summary>
        [JsonProperty("contextId")]
        public string ContextId { get; set; }

        /// <summary>
        /// Get or set the array of <see cref="LineItem"/>'s in the LineItemMembershipSubject.
        /// </summary>
        [JsonProperty("lineItem")]
        public LineItem[] LineItems { get; set; }
    }
}
