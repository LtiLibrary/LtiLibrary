using LtiLibrary.NetCore.Profiles;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Profiles
{
    public class GetToolConsumerProfileContext
    {
        public GetToolConsumerProfileContext(string ltiVersion)
        {
            LtiVersion = ltiVersion;
            StatusCode = StatusCodes.Status200OK;
        }

        public string LtiVersion { get; }
        public int StatusCode { get; set; }
        public ToolConsumerProfile ToolConsumerProfile { get; set; }
    }
}
