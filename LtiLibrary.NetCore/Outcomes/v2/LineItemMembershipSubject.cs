using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    public class LineItemMembershipSubject : JsonLdObject
    {
        [JsonProperty("contextId")]
        public string ContextId { get; set; }

        [JsonProperty("lineItem")]
        public LineItem[] LineItems { get; set; }
    }
}
