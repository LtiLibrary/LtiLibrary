using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    /// <summary>
    /// An ResultContainer defines the endpoint to which clients POST new LisResult resources 
    /// and from which they GET the list of Results associated with a a given learning context.
    /// </summary>
    public class ResultContainer : JsonLdObject
    {
        /// <summary>
        /// Initializes a new instance of the ResultContainer class.
        /// </summary>
        public ResultContainer()
        {
            Type = LtiConstants.LisResultContainerType;
        }

        /// <summary>
        /// Optional indication of which resource is the subject for the members of the container.
        /// </summary>
        [JsonProperty("membershipSubject")]
        public ResultMembershipSubject MembershipSubject { get; set; }
    }
}
