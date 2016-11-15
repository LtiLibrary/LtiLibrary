using System;
using System.Net;

namespace LtiLibrary.Core.Outcomes.v2
{
    public class OutcomeResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Exception is case of failed request.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// String representation of the HttpWebRequest similar to Fiddler's.
        /// </summary>
        public string HttpRequest { get; set; }

        /// <summary>
        /// String representation of the HttpWebResponse similar to Fiddler's.
        /// </summary>
        public string HttpResponse { get; set; }
    }

    public class OutcomeResponse<T> : OutcomeResponse where T : class 
    {
        public T Outcome { get; set; }
    }
}
