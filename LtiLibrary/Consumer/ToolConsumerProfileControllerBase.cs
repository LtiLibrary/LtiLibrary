using System.Web.Http;

namespace LtiLibrary.Consumer
{
    /// <summary>
    /// Implements the LTI Tool Consumer Profile API.
    /// </summary>
    [ToolConsumerProfileControllerConfig]
    public abstract class ToolConsumerProfileControllerBase : ApiController
    {
        [HttpGet]
// ReSharper disable InconsistentNaming
        public abstract ToolConsumerProfile Get(string lti_version = "LTI-1p0");
// ReSharper restore InconsistentNaming
    }
}
