using System.Net;

namespace LtiLibrary.Core.Profiles
{
    public class ToolConsumerProfileResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public ToolConsumerProfile ToolConsumerProfile { get; set; }
    }
}
