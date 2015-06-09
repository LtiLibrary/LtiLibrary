using System;
using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.Core.Lti2
{
    /// <summary>
    /// This resource represents the service provider that hosts a product (Tool or Tool Consumer).
    /// </summary>
    public class ServiceProvider : JsonLdObject
    {
        /// <summary>
        /// A description of the service provider suitable for display to end-users.
        /// </summary>
        [JsonProperty("description")]
        public ServiceProviderDescription Description { get; set; }

        /// <summary>
        /// A globally unique identifier for the service provider. As a best practice, this value should match an 
        /// Internet domain name assigned by ICANN, but any globally unique identifier is acceptable.
        /// </summary>
        [JsonProperty("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// The name of the service provider suitable for display to end users.
        /// </summary>
        [JsonProperty("service_provider_name")]
        public ServiceProviderName ServiceProviderName { get; set; }

        /// <summary>
        /// Contact information for support from a service provider.
        /// </summary>
        [JsonProperty("support")]
        public Contact Support { get; set; }

        /// <summary>
        /// A timestamp for the Service Provider record. This value is useful for determining which record is most current.
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
