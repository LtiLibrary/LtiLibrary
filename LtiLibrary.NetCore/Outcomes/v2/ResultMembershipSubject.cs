using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    /// <summary>
    /// Represents an IMS ResultMembershipSubject object.
    /// </summary>
    public class ResultMembershipSubject : JsonLdObject
    {
        /// <summary>
        /// Get or set the array of <see cref="LisResult"/>'s.
        /// </summary>
        [JsonProperty("result")]
        public LisResult[] Results { get; set; }
    }
}
