using System;
using System.Net;

namespace LtiLibrary.NetCore.Common
{
    /// <summary>
    /// Encapsulates the client response to API calls such as <see cref="Clients.Outcomes1Client.DeleteResultAsync"/>.
    /// </summary>
    public class ClientResponse
    {
        /// <summary>
        /// The <see cref="HttpStatusCode"/> from the underlying API call.
        /// </summary>
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
#endif
        /// <summary>
        /// String representation of the HttpWebResponse similar to Fiddler's.
        /// </summary>
        public string HttpResponse { get; set; }
    }

    /// <summary>
    /// Represents a <see cref="ClientResponse"/> that also contains a Response of type T.
    /// </summary>
    /// <typeparam name="T">The type of the Response object that is in the client response.</typeparam>
    public class ClientResponse<T> : ClientResponse where T : class 
    {
        /// <summary>
        /// Get or Set the Response.
        /// </summary>
        public T Response { get; set; }
    }
}
