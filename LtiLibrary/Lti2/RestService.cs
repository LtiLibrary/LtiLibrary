using System;
using System.Runtime.Serialization;

namespace LtiLibrary.Lti2
{
    /// <summary>
    /// A descriptor for a REST service.
    /// </summary>
    [DataContract]
    public class RestService
    {
        public RestService()
        {
            Type = "RestService";
        }

        /// <summary>
        /// A URI which identifies this service instance.
        /// </summary>
        [DataMember(Name = "@id")]
        public string Id { get; set; }

        /// <summary>
        /// The type of service.
        /// </summary>
        [DataMember(Name = "@type")]
        public string Type { get; private set; }

        /// <summary>
        /// Specifies the HTTP actions that are supported at the REST endpoint. A given endpoint may support more 
        /// than one action. For example, it is common practice for a collection endpoint to accept POST requests, 
        /// and a different item endpoint to accept GET, PUT, and DELETE requests.
        /// </summary>
        [DataMember(Name = "action")]
        public string[] Action { get; set; }

        /// <summary>
        /// A URI template that defines the REST endpoint for resources that may be accessed via the REST service. 
        /// The URI template should conform to the syntax in the proposed IETF standard.
        /// </summary>
        [DataMember(Name = "endpoint")]
        public Uri EndPoint { get; set; }

        /// <summary>
        /// The content type (also known as media type) of resources accessed via the endpoint defined by this
        /// RestService descriptor. The endpoint of a REST service may support more than one content type if 
        /// content negotiation is enabled as described in the HTTP 1.1 specification.
        /// </summary>
        [DataMember(Name = "format")]
        public string[] Format { get; set; }
    }
}
