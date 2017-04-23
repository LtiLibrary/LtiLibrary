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
        /// <summary>
        /// Initialize a new instance of the ToolConsumerProfileControllerBase class.
        /// </summary>
        protected ToolConsumerProfileControllerBase()
        {
            OnGetToolConsumerProfile = dto => throw new NotImplementedException();
        }

        /// <summary>
        /// Return the ToolConsumerProfile.
        /// </summary>
        public Func<GetToolConsumerProfileDto, Task> OnGetToolConsumerProfile { get; set; }

        /// <summary>
        /// Get the <see cref="NetCore.Profiles.ToolConsumerProfile"/>
        /// </summary>
        /// <param name="lti_version">The LTI version of the tool provider making the request. Defaults to "LTI-1p0".</param>
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
