using System;
using System.Threading.Tasks;
using LtiLibrary.NetCore.Common;
using LtiLibrary.NetCore.Lti.v2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNetCore.Profiles
{
    /// <summary>
    /// Implements the LTI Tool Consumer Profile API.
    /// </summary>
    /// <remarks>
    /// Unless it is overridden, the route for this controller will be "ims/[controller]" named "ToolConsumerProfileApi".
    /// </remarks>
    [Route("ims/[controller]", Name = "ToolConsumerProfileApi")]
    [Consumes(LtiConstants.LtiToolConsumerProfileMediaType)]
    [Produces(LtiConstants.LtiToolConsumerProfileMediaType)]
    public abstract class ToolConsumerProfileControllerBase : Controller
    {
        /// <summary>
        /// Return the ToolConsumerProfile.
        /// </summary>
        protected abstract Func<GetToolConsumerProfileRequest, Task<GetToolConsumerProfileResponse>> OnGetToolConsumerProfileAsync { get; }

        /// <summary>
        /// Get the <see cref="ToolConsumerProfile"/>
        /// </summary>
        /// <param name="lti_version">The LTI version of the tool provider making the request. Defaults to "LTI-1p0".</param>
        [HttpGet]
// ReSharper disable InconsistentNaming
        public async Task<IActionResult> GetAsync(string lti_version = "LTI-1p0")
// ReSharper restore InconsistentNaming
        {
            try
            {
                if (OnGetToolConsumerProfileAsync == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                // Invoke OnGetToolConsumerProfileAsync in the application's controller to fill in the profile
                var request = new GetToolConsumerProfileRequest(lti_version);
                var response = await OnGetToolConsumerProfileAsync(request);

                // Return the result
                if (response.StatusCode == StatusCodes.Status200OK)
                {
                    return new ToolConsumerProfileResult(response.ToolConsumerProfile);
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
