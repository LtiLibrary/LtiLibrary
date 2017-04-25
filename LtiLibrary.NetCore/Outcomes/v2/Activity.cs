using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    /// <summary>
    /// A resource that a person may experience such as a video or an assessment.
    /// This entity represents the resource itself, not the person's engagement with the resource.
    /// </summary>
    public class Activity : JsonLdObject
    {
        /// <summary>
        /// Initializes a new instance of the Activity class.
        /// </summary>
        public Activity()
        {
            Type = LtiConstants.ActivityType;
        }

        /// <summary>
        /// The unique ID for the activity as used by the tool provider.
        /// </summary>
        [JsonProperty("activityId")]
        public string ActivityId { get; set; }
    }
}
