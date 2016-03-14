using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.Core.Outcomes.v2
{
    public class LineItemMembershipSubject : JsonLdObject
    {
        [JsonProperty("contextId")]
        public string ContextId { get; set; }

        [JsonProperty("lineItem")]
        public LineItem[] LineItems { get; set; }
    }
}
