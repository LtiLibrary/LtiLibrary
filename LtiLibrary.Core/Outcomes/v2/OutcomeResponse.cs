using System.Net;

namespace LtiLibrary.Core.Outcomes.v2
{
    public class OutcomeResponse<T> where T: class 
    {
        public T Outcome { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// String representation of the HttpWebRequest similar to Fiddler's.
        /// </summary>
        public string HttpRequest { get; set; }

        /// <summary>
        /// String representation of the HttpWebResponse similar to Fiddler's.
        /// </summary>
        public string HttpResponse { get; set; }
    }
}
