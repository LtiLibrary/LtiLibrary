using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lis.v2;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    /// <summary>
    /// Represents an IMS ResultMembershipSubject object.
    /// </summary>
    public class ResultMembershipSubject : JsonLdObject
    {
        /// <summary>
        /// Get or set the array of <see cref="Result"/>'s.
        /// </summary>
        [JsonProperty("result")]
        public Result[] Results { get; set; }
    }
}
