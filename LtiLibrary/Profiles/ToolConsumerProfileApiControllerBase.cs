using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LtiLibrary.Profiles
{
    /// <summary>
    /// Implements the LTI Tool Consumer Profile API.
    /// </summary>
    [ToolConsumerProfileControllerConfig]
    public abstract class ToolConsumerProfileApiControllerBase : ApiController
    {
        protected abstract ToolConsumerProfile GetToolConsumerProfile(string ltiVersion);

        [HttpGet]
// ReSharper disable InconsistentNaming
        public HttpResponseMessage Get(string lti_version = "LTI-1p2")
// ReSharper restore InconsistentNaming
        {
            var profile = GetToolConsumerProfile(lti_version);
            return profile == null 
                ? Request.CreateResponse(HttpStatusCode.NotImplemented) 
                : Request.CreateResponse(HttpStatusCode.OK, profile);
        }
    }
}
