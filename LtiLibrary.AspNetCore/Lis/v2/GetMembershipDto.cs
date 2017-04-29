using LtiLibrary.NetCore.Lis.v2;
using LtiLibrary.NetCore.Lti1;

namespace LtiLibrary.AspNetCore.Lis.v2
{
    public class GetMembershipDto
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public GetMembershipDto(int? limit, string rlid, Role? role, int? page)
        {
            Limit = limit;
            Rlid = rlid;
            Role = role;
            Page = page;
        }

        /// <summary>
        /// Specifies the maximum number of items that should be delivered per page. This parameter is merely a hint. The server is not obligated to honor this limit and may at its own discretion choose a different value for the number of items per page.
        /// </summary>
        public int? Limit { get; }
        
        /// <summary>
        /// Get or set the MembershipContainerPage.
        /// </summary>
        public MembershipContainerPage MembershipContainerPage { get; set; }
        
        /// <summary>
        /// Get or set the page #.
        /// </summary>
        public int? Page { get; }

        /// <summary>
        /// The ID of a resource link within the context and associated and the Tool Provider. The result set will be filtered so that it includes only those memberships that are permitted to access the resource link. If omitted, the result set will include all memberships for the context.
        /// </summary>
        public string Rlid { get; }

        /// <summary>
        /// The role for a membership. The result set will be filtered so that it includes only those memberships that contain this role. The value of the parameter should be the full URI for the role, although the simple name may be used for context-level roles. If omitted, the result set will include all memberships with any role.
        /// </summary>
        public Role? Role { get; }

        /// <summary>
        /// Get or set the HTTP status code.
        /// </summary>
        public int StatusCode { get; set; }
    }
}
