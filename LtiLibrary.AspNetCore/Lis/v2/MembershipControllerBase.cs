using System;
using System.Threading.Tasks;
using LtiLibrary.AspNetCore.Common;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Lis.v2
{
    /// <summary>
    /// A <see cref="Controller" /> that implements a REST API for reading LISMembershipContainer resources
    /// https://www.imsglobal.org/lti/model/uml/purl.imsglobal.org/vocab/lis/v2/mm/LISMembershipContainer/service.html
    /// </summary>
    [AddBodyHashHeader]
    [Route("ims/membership", Name = "MembershipApi")]
    [Consumes(LtiConstants.LisMembershipContainerMediaType)]
    [Produces(LtiConstants.LisMembershipContainerMediaType)]
    public abstract class MembershipControllerBase : Controller
    {
        /// <summary>
        /// Initializes a new instance of the LineItemsControllerBase class.
        /// </summary>
        protected MembershipControllerBase()
        {
            OnGetMembership = dto => throw new NotImplementedException();
        }

        /// <summary>
        /// Get a list of memberships.
        /// </summary>
        public Func<GetMembershipDto, Task> OnGetMembership { get; set; }

        /// <summary>
        /// Get a list of memberships.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAsync(int? limit = null, string rlid = null, Role? role = null, int? page = null)
        {
            try
            {
                var membershipsDto = new GetMembershipDto(limit, rlid, role, page);
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
