using System;
using System.Threading.Tasks;
using LtiLibrary.Core.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiLibrary.AspNet.Profiles
{
    /// <summary>
    /// Implements the LTI Tool Consumer Profile API.
    /// </summary>
    [Consumes(LtiConstants.ToolConsumerProfileMediaType)]
    public abstract class ToolConsumerProfileControllerBase : Controller
    {
        protected ToolConsumerProfileControllerBase()
        {
            OnGetToolConsumerProfile = context => { throw new NotImplementedException(); };
        }

        public Func<GetToolConsumerProfileContext, Task> OnGetToolConsumerProfile { get; set; }

        [HttpGet]
// ReSharper disable InconsistentNaming
        public async Task<IActionResult> GetAsync(string lti_version = "LTI-1p0")
// ReSharper restore InconsistentNaming
        {
            try
            {
                var context = new GetToolConsumerProfileContext(lti_version);

                await OnGetToolConsumerProfile(context);

                if (context.StatusCode == StatusCodes.Status200OK)
                {
                    return new ToolConsumerProfileResult(context.ToolConsumerProfile);
                }
                else
                {
                    return StatusCode(context.StatusCode);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
