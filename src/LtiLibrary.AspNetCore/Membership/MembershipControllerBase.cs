﻿using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Common;
using LtiLibrary.AspNetCore.Extensions;
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
    /// Unless it is overridden, the route for this controller will be "ims/[controller]/context/{contextid}" named "MembershipApi".
    /// </remarks>
    [AddBodyHashHeader]
    [Route("ims/[controller]/context/{contextid}", Name = "MembershipApi")]
    [Consumes(LtiConstants.LisMembershipContainerMediaType)]
    [Produces(LtiConstants.LisMembershipContainerMediaType)]
    public abstract class MembershipControllerBase : Controller
    {
        /// <summary>
        /// Populate the <see cref="GetMembershipResponse"/> with the membership and set the StatusCode
        /// to signify success or failure.
        /// </summary>
        protected abstract Func<GetMembershipRequest, Task<GetMembershipResponse>> OnGetMembershipAsync { get; }

        /// <summary>
        /// To get a representation of a particular LISMembershipContainer instance, the client submits an HTTP GET 
        /// request to the resource's REST endpoint. The contextId is embedded in the endpoint URL.
        /// The request may contain the query parameters specified in Table 1:
        /// https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/mm/LISMembershipContainer/service.html#queryParams.
        /// The request must contain the headers in Table 2: 
        /// https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/mm/LISMembershipContainer/service.html#getHeader.
        /// </summary>
        /// <param name="contextId">The LTI context_id of the context from which to pull the membership.</param>
        /// <param name="limit">Optional. Specifies the maximum number of items that should be delivered per page. This parameter is merely a hint. The server is not obligated to honor this limit and may at its own discretion choose a different value for the number of items per page.</param>
        /// <param name="rlid">Optional. The ID of a resource link within the context and associated and the Tool Provider. The result set will be filtered so that it includes only those memberships that are permitted to access the resource link. If omitted, the result set will include all memberships for the context.</param>
        /// <param name="role">Optional. The role for a membership. The result set will be filtered so that it includes only those memberships that contain this role. The value of the parameter should be the full URI for the role, although the simple name may be used for context-level roles. If omitted, the result set will include all memberships with any role.</param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> GetAsync(string contextId, int? limit = null, string rlid = null, Role? role = null)
        {
            try
            {
                // Check for required Authorization header with parameters dictated by the OAuth Body Hash Protocol
                if (!Request.IsAuthenticatedWithLti())
                {
                    return Unauthorized();
                }

                if (OnGetMembershipAsync == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                if (string.IsNullOrEmpty(contextId))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, $"{nameof(contextId)} is null or empty.");
                }

                // Invoke OnGetMembershipAsync in the application's controller to fill in the membership
                var request = new GetMembershipRequest(contextId, limit, rlid, role);
                var response = await OnGetMembershipAsync(request);

                // Return the result
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return new MembershipContainerPageResult(response.MembershipContainerPage);
                }
                return StatusCode(response.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
