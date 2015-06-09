using System;
using LtiLibrary.Core.Common;
using Newtonsoft.Json;

namespace LtiLibrary.Core.Lti2
{
    /// <summary>
    /// A person or agency that offers products to the market. The key types of products relevant to LTI 
    /// are Tools (which are described by Tool Profiles) and Tool Consumers (which are described by Tool 
    /// Consumer Profiles).
    /// </summary>
    public class Vendor : JsonLdObject
    {
        /// <summary>
        /// A unique identifier for the resource.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Contact information for this Vendor.
        /// </summary>
        [JsonProperty("contact")]
        public Contact Contact { get; set; }

        /// <summary>
        /// This is a human-readable description of the Vendor.
        /// </summary>
        [JsonProperty("description")]
        public LocalizedText Description { get; set; }

        /// <summary>
        /// A timestamp for the Vendor record. This value is useful for determining which record is most current.
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Defines a human readable name for the Vendor. The name should be suitable for display in management screens within the Tool Provider system.
        /// </summary>
        [JsonProperty("vendor_name")]
        public LocalizedName VendorName { get; set; }

        /// <summary>
        /// This is the URL of the vendor.
        /// </summary>
        [JsonProperty("website")]
        public string Website { get; set; }
    }
}
