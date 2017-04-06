using System;
using System.Net;

namespace LtiLibrary.NetCore.Outcomes.v2
{
    public class ClientResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Exception is case of failed request.
        /// </summary>
        public Exception Exception { get; set; }
#if DEBUG
        /// <summary>
        /// String representation of the HttpWebRequest similar to Fiddler's.
        /// </summary>
        public string HttpRequest { get; set; }

        /// <summary>
        /// String representation of the HttpWebResponse similar to Fiddler's.
        /// </summary>
        public string HttpResponse { get; set; }
#endif
    }

    public class ClientResponse<T> : ClientResponse where T : class 
    {
        public T Response { get; set; }
    }
}
