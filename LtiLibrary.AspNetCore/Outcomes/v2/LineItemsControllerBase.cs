using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Common;
using LtiLibrary.AspNetCore.Lis.v2;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Outcomes.v2
{
    /// <summary>
    /// A <see cref="Controller" /> that implements a REST API for reading LISMembershipContainer resources
    /// https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/mm/LISMembershipContainer/service.html
    /// </summary>
    [AddBodyHashHeader]
    [Route("ims/memberships", Name = "MembershipsApi")]
    [Consumes(LtiConstants.LisMembershipContainerMediaType)]
    [Produces(LtiConstants.LisMembershipContainerMediaType)]
    public abstract class LineItemsControllerBase : Controller
    {
        /// <summary>
        /// Initializes a new instance of the LineItemsControllerBase class.
        /// </summary>
        protected LineItemsControllerBase()
        {
            OnGetMemberships = dto => throw new NotImplementedException();
        }

        /// <summary>
        /// Get a list of memberships.
        /// </summary>
        public Func<GetMembershipsDto, Task> OnGetMemberships { get; set; }

        /// <summary>
        /// Get a list of memberships.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync(int? limit = null, string rlid = null, Role? role = null)
        {
            try
            {
                var membershipsDto = new GetMembershipsDto(limit, rlid, role);
                await OnGetMemberships(membershipsDto);
                if (membershipsDto.StatusCode == StatusCodes.Status200OK)
                {
                    return new MembershipContainerPageResult(membershipsDto);
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

