using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.Core.Outcomes.v2
{
    /// <summary>
    /// An LisResultContainer defines the endpoint to which clients POST new LisResult resources 
    /// and from which they GET the list of LisResults associated with a a given learning context.
    /// </summary>
    public class LisResultContainer : JsonLdObject
    {
        public LisResultContainer()
        {
            Type = LtiConstants.LisResultContainerType;
        }

        /// <summary>
        /// Optional indication of which resource is the subject for the members of the container.
        /// </summary>
        [JsonProperty("membershipSubject")]
        public Context MembershipSubject { get; set; }
    }
}
