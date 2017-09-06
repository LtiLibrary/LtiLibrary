using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lis.v2
{
    /// <summary>
    /// A MembershipContainer defines the endpoint to which clients GET the list of Memberships associated with a a given organization, such as a course.
    /// This specification document describes a media type suitable for the response from a GET request. The response is based on the W3C Linked Data Platform 
    /// recommendations.
    /// </summary>
    public class MembershipContainer : JsonLdObject
    {
        /// <summary>
        /// Initializes a new instance of the LineItemContainer class.
        /// </summary>
        public MembershipContainer()
        {
            Type = LtiConstants.LisMembershipContainerType;
        }

        /// <summary>
        /// Optional indication of which resource is the subject for the members of the container.
        /// </summary>
        [JsonProperty("membershipSubject")]
        public Context MembershipSubject { get; set; }
    }
}
