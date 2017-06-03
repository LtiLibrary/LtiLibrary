using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Common;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lis.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Membership
{
    /// <summary>
    /// A <see cref="Controller" /> that implements a REST API for reading LISMembershipContainer resources
    /// https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/mm/LISMembershipContainer/service.html
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default the Route for this controller will be "ims/[controller]" named "MembershipApi".
    /// </para>
    /// </remarks>
    [AddBodyHashHeader]
    [Route("ims/[controller]", Name = "MembershipApi")]
    [Consumes(LtiConstants.LisMembershipContainerMediaType)]
    [Produces(LtiConstants.LisMembershipContainerMediaType)]
    public abstract class MembershipControllerBase : Controller
    {
        /// <summary>
        /// Implement this method to get a list of memberships
        /// </summary>
        protected abstract Func<GetMembershipDto, Task> OnGetMembership { get; }

        /// <summary>
        /// To get a representation of a particular LISMembershipContainer instance, the client submits an HTTP GET 
        /// request to the resource's REST endpoint, in accordance with the following rules:
        /// The request may contain the query parameters specified in Table 1 (https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/mm/LISMembershipContainer/service.html#queryParams).
        /// </summary>
        /// <param name="limit">Specifies the maximum number of items that should be delivered per page. This parameter is merely a hint. The server is not obligated to honor this limit and may at its own discretion choose a different value for the number of items per page.</param>
        /// <param name="rlid">The ID of a resource link within the context and associated and the Tool Provider. The result set will be filtered so that it includes only those memberships that are permitted to access the resource link. If omitted, the result set will include all memberships for the context.</param>
        /// <param name="role">The role for a membership. The result set will be filtered so that it includes only those memberships that contain this role. The value of the parameter should be the full URI for the role, although the simple name may be used for context-level roles. If omitted, the result set will include all memberships with any role.</param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> GetAsync(int? limit = null, string rlid = null, Role? role = null)
        {
            try
            {
                if (OnGetMembership == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                var membershipsDto = new GetMembershipDto(limit, rlid, role);
                await OnGetMembership(membershipsDto);
                if (membershipsDto.StatusCode == StatusCodes.Status200OK)
                {
                    return new MembershipContainerPageResult(membershipsDto.MembershipContainerPage);
                }
                return StatusCode(membershipsDto.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
