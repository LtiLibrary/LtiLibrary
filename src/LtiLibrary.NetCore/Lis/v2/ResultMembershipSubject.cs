using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lis.v2
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
