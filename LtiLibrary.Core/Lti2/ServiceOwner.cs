using System;
using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.Core.Lti2
{
    public class ServiceOwner : JsonLdObject
    {
        public ServiceOwner()
        {
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// A timestamp for the Service Owner record. This value is useful for determining which record is most current.
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// A description of the service owner suitable for display to end-users.
        /// Should match tool_consumer_instance_description.
        /// </summary>
        [JsonProperty("description")]
        public ServiceOwnerDescription Description { get; set; }

        /// <summary>
        /// Should match tool_consumer_instance_name launch parameter.
        /// </summary>
        [JsonProperty("service_owner_name")]
        public ServiceOwnerName Name { get; set; }

        /// <summary>
        /// Should match tool_consumer_instance_contact_email launch parameter.
        /// </summary>
        [JsonProperty("support")]
        public Contact Support { get; set; }
    }
}
