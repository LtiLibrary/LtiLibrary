using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti1;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lis.v2
{
    /// <summary>
    /// Indicates the nature of an Agent's membership within an Organization.
    /// </summary>
    public class Membership
    {
        /// <summary>
        /// Indicates the Person (or other Agent including Organization) involved in the Membership relationship.
        /// </summary>
        [JsonProperty("member")]
        public Person Member { get; set; }

        /// <summary>
        /// A copy of any parameters which would be included in a message initiated by the person and whose value includes data specific to them. 
        /// Any custom parameters are included in an element named "custom" and have the "custom_" prefix removed from their name. Similarly any 
        /// extension parameters are placed in an element named "ext" and the "ext_" prefix is removed from their name.
        /// </summary>
        [JsonProperty("message")]
        public object Message { get; set; }

        /// <summary>
        /// Indicates the Roles that the Agent plays in a Membership relationship with an Organization.
        /// </summary>
        [JsonProperty("role")]
        public Role[] Role { get; set; }

        /// <summary>
        /// The current status of a membership which applies to all roles.
        /// </summary>
        [JsonProperty("status")]
        public Status Status { get; set; }
    }
}
