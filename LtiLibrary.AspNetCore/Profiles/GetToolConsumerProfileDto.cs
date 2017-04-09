using LtiLibrary.NetCore.Profiles;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNetCore.Profiles
{
    public class GetToolConsumerProfileDto
    {
        public GetToolConsumerProfileDto(string ltiVersion)
        {
            LtiVersion = ltiVersion;
            StatusCode = StatusCodes.Status200OK;
        }

        public string LtiVersion { get; }
        public int StatusCode { get; set; }
        public ToolConsumerProfile ToolConsumerProfile { get; set; }
    }
}
