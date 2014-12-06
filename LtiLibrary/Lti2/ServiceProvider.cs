using System;
using System.Runtime.Serialization;

namespace LtiLibrary.Lti2
{
    /// <summary>
    /// This resource represents the service provider that hosts a product (Tool or Tool Consumer).
    /// </summary>
    [DataContract]
    public class ServiceProvider
    {
        public ServiceProvider(string guid, string providerName)
        {
            Guid = guid;
            ServiceProviderName = new LocalizedName{Key = "provider_name", Value = providerName};
            Timestamp = DateTime.UtcNow;
        }

        [DataMember(Name = "@id")]
        public string Id { get; set; }

        /// <summary>
        /// A description of the service provider suitable for display to end-users.
        /// </summary>
        [DataMember(Name = "description")]
        public LocalizedText Description { get; set; }

        /// <summary>
        /// A globally unique identifier for the service provider. As a best practice, this value should match an 
        /// Internet domain name assigned by ICANN, but any globally unique identifier is acceptable.
        /// </summary>
        [DataMember(Name = "guid")]
        public string Guid { get; set; }

        /// <summary>
        /// The name of the service provider suitable for display to end users.
        /// </summary>
        [DataMember(Name = "provider_name")]
        public LocalizedName ServiceProviderName { get; private set; }

        /// <summary>
        /// Contact information for support from a service provider.
        /// </summary>
        [DataMember(Name = "support")]
        public Contact Support { get; set; }

        /// <summary>
        /// A timestamp for the Service Provider record. This value is useful for determining which record is most current.
        /// </summary>
        [DataMember(Name = "timestamp")]
        public DateTime Timestamp { get; private set; }
    }
}
