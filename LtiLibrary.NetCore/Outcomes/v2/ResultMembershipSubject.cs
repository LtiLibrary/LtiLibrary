using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    public class ResultMembershipSubject : JsonLdObject
    {
        [JsonProperty("result")]
        public LisResult[] Results { get; set; }
    }
}
