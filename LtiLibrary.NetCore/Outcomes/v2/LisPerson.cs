using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    /// <summary>
    /// Represents an IMS LisPerson object.
    /// </summary>
    public class LisPerson : JsonLdObject
    {
        /// <summary>
        /// A unique identifier for the person.
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
