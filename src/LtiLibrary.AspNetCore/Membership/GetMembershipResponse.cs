using LtiLibrary.NetCore.Lis.v2;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Membership
{
    /// <summary>
    /// Represents a GetMembership result.
    /// </summary>
    public class GetMembershipResponse
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public GetMembershipResponse()
        {
            StatusCode = StatusCodes.Status200OK;
        }
        
        /// <summary>
        /// Get or set the MembershipContainerPage.
        /// </summary>
        public MembershipContainerPage MembershipContainerPage { get; set; }

        /// <summary>
        /// Get or set the HTTP status code representing the success or failure of the getting the membership.
        /// </summary>
        public int StatusCode { get; set; }
    }
}
