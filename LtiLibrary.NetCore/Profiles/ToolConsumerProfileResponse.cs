using System.Net;

namespace LtiLibrary.NetCore.Profiles
{
    public class ToolConsumerProfileResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public ToolConsumerProfile ToolConsumerProfile { get; set; }
    }
}
