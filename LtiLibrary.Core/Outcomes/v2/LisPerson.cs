using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.Core.Outcomes.v2
{
    public class LisPerson : JsonLdObject
    {
        /// <summary>
        /// A unique identifier for the person.
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
