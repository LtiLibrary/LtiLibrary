using System.Net;

namespace LtiLibrary.NetCore.Profiles
{
    public class ToolConsumerProfileResponse
    {
        public string ContentType { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public ToolConsumerProfile ToolConsumerProfile { get; set; }
    }
}
