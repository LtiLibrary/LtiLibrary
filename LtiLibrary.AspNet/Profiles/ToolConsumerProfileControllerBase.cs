using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace LtiLibrary.AspNet.Profiles
{
    /// <summary>
    /// Implements the LTI Tool Consumer Profile API.
    /// </summary>
    [ToolConsumerProfileControllerConfig]
    public abstract class ToolConsumerProfileControllerBase : ApiController
    {
        protected ToolConsumerProfileControllerBase()
        {
            OnGetToolConsumerProfile = context => { throw new NotImplementedException(); };
        }

        public Func<GetToolConsumerProfileContext, Task> OnGetToolConsumerProfile { get; set; }

        [HttpGet]
// ReSharper disable InconsistentNaming
        public async Task<HttpResponseMessage> Get(string lti_version = "LTI-1p0")
// ReSharper restore InconsistentNaming
        {
            try
            {
                var context = new GetToolConsumerProfileContext(lti_version);

                await OnGetToolConsumerProfile(context);
                
                return context.StatusCode == HttpStatusCode.OK
                    ? Request.CreateResponse(context.StatusCode, context.ToolConsumerProfile)
                    : Request.CreateResponse(context.StatusCode);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
