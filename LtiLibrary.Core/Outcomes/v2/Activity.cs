using System.Runtime.Serialization;
using LtiLibrary.Core.Common;

namespace LtiLibrary.Core.Outcomes.v2
{
    /// <summary>
    /// A resource that a person may experience such as a video or an assessment.
    /// This entity represents the resource itself, not the person's engagement with the resource.
    /// </summary>
    [DataContract]
    public class Activity : JsonLdObject
    {
        public Activity() : base(LtiConstants.ActivityType)
        {
        }

        /// <summary>
        /// The unique ID for the activity as used by the tool provider.
        /// </summary>
        [DataMember(Name = "activity_id")]
        public string ActivityId { get; set; }
    }
}
