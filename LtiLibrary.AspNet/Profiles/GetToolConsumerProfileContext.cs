using System.Net;
using LtiLibrary.Core.Profiles;

namespace LtiLibrary.AspNet.Profiles
{
    public class GetToolConsumerProfileContext
    {
        public GetToolConsumerProfileContext(string ltiVersion)
        {
            LtiVersion = ltiVersion;
            StatusCode = HttpStatusCode.OK;
        }

        public string LtiVersion { get; private set; }
        public HttpStatusCode StatusCode { get; set; }
        public ToolConsumerProfile ToolConsumerProfile { get; set; }
    }
}
