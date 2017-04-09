using System;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Profiles
{
    /// <summary>
    /// Implements the LTI Tool Consumer Profile API.
    /// </summary>
    [Route("ims/toolconsumerprofile", Name = "ToolConsumerProfileApi")]
    [Consumes(LtiConstants.ToolConsumerProfileMediaType)]
    [Produces(LtiConstants.ToolConsumerProfileMediaType)]
    public abstract class ToolConsumerProfileControllerBase : Controller
    {
        protected ToolConsumerProfileControllerBase()
        {
            OnGetToolConsumerProfile = dto => throw new NotImplementedException();
        }

        public Func<GetToolConsumerProfileDto, Task> OnGetToolConsumerProfile { get; set; }

        [HttpGet]
// ReSharper disable InconsistentNaming
        public async Task<IActionResult> GetAsync(string lti_version = "LTI-1p0")
// ReSharper restore InconsistentNaming
        {
            try
            {
                var dto = new GetToolConsumerProfileDto(lti_version);

                await OnGetToolConsumerProfile(dto);

                if (dto.StatusCode == StatusCodes.Status200OK)
                {
                    // Set the Content-Type of the ObjectResult
                    return new ToolConsumerProfileResult(dto.ToolConsumerProfile);
                }
                return StatusCode(dto.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
