using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    /// <summary>
    /// A LineItemContainer defines the endpoint to which clients POST new LineItem resources 
    /// and from which they GET the list of LineItems associated with a a given learning context.
    /// </summary>
    public class LineItemContainer : JsonLdObject
    {
        /// <summary>
        /// Initializes a new instance of the LineItemContainer class.
        /// </summary>
        public LineItemContainer()
        {
            Type = LtiConstants.LineItemContainerType;
        }

        /// <summary>
        /// Optional indication of which resource is the subject for the members of the container.
        /// </summary>
        [JsonProperty("membershipSubject")]
        public LineItemMembershipSubject MembershipSubject { get; set; }
    }
}
