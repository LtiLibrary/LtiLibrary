using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.Core.Outcomes.v2
{
    /// <summary>
    /// A LineItemContainer defines the endpoint to which clients POST new LineItem resources 
    /// and from which they GET the list of LineItems associated with a a given learning context.
    /// </summary>
    public class LineItemContainer : JsonLdObject
    {
        public LineItemContainer()
        {
            Type = LtiConstants.LineItemContainerType;
        }

        /// <summary>
        /// Optional indication of which resource is the subject for the members of the container.
        /// </summary>
        [JsonProperty("membershipSubject")]
        public Context MembershipSubject { get; set; }
    }
}
