using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.Core.Outcomes.v2
{
    public class ResultMembershipSubject : JsonLdObject
    {
        [JsonProperty("result")]
        public LisResult[] Results { get; set; }
    }
}
