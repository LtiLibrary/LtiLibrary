using System.Net;
using LtiLibrary.Core.Profiles;
using Microsoft.AspNetCore.Http;

namespace LtiLibrary.AspNet.Profiles
{
    public class GetToolConsumerProfileContext
    {
        public GetToolConsumerProfileContext(string ltiVersion)
        {
            LtiVersion = ltiVersion;
            StatusCode = StatusCodes.Status200OK;
        }

        public string LtiVersion { get; private set; }
        public int StatusCode { get; set; }
        public ToolConsumerProfile ToolConsumerProfile { get; set; }
    }
}
