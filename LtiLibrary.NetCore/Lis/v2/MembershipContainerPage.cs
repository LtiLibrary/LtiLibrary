using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lis.v2
{
    /// <summary>
    /// Represents a page of MembershipContainer.
    /// </summary>
    public class MembershipContainerPage : JsonLdObject
    {
        /// <summary>
        /// Initializes a new instance of the MembershipContainerPage class.
        /// </summary>
        public MembershipContainerPage()
        {
            Type = LtiConstants.PageType;
        }

        /// <summary>
        /// Optional URI for a change history that identifies added and deleted members.
        /// </summary>
        [JsonProperty("differences")]
        public string Differences { get; set; }

        /// <summary>
        /// URI for the next page. If there is no next page, the NextPage property will be null.
        /// </summary>
        [JsonProperty("nextPage")]
        public string NextPage { get; set; }

        /// <summary>
        /// The Memberships within this page.
        /// </summary>
        [JsonProperty("pageOf")]
        public MembershipContainer MembershipContainer { get; set; }
    }
}
