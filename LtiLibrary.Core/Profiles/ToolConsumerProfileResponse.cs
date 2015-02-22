using System.Net;

namespace LtiLibrary.Core.Profiles
{
    public class ToolConsumerProfileResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// String representation of the HttpWebRequest similar to Fiddler's.
        /// </summary>
        public string HttpRequest { get; set; }

        /// <summary>
        /// String representation of the HttpWebResponse similar to Fiddler's.
        /// </summary>
        public string HttpResponse { get; set; }

        public ToolConsumerProfile ToolConsumerProfile { get; set; }
    }
}
