using System;
using System.Runtime.Serialization;

namespace LtiLibrary.Lti2
{
    /// <summary>
    /// A person or agency that offers products to the market. The key types of products relevant to LTI 
    /// are Tools (which are described by Tool Profiles) and Tool Consumers (which are described by Tool 
    /// Consumer Profiles).
    /// </summary>
    [DataContract]
    public class Vendor
    {
        public Vendor(string code, string vendorName)
        {
            Code = code;
            Timestamp = DateTime.UtcNow;
            VendorName = new LocalizedName {Key = "product.vendor.name", Value = vendorName};
        }

        /// <summary>
        /// The URI that identifies this Vendor instance.
        /// </summary>
        [DataMember(Name = "@id")]
        public string Id { get; set; }

        /// <summary>
        /// A unique identifier for the resource.
        /// </summary>
        [DataMember(Name = "code")]
        public string Code { get; private set; }

        /// <summary>
        /// Contact information for this Vendor.
        /// </summary>
        [DataMember(Name = "contact")]
        public Contact Contact { get; set; }

        /// <summary>
        /// This is a human-readable description of the Vendor.
        /// </summary>
        [DataMember(Name = "description")]
        public LocalizedText Description { get; set; }

        /// <summary>
        /// A timestamp for the Vendor record. This value is useful for determining which record is most current.
        /// </summary>
        [DataMember(Name = "timestamp")]
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Defines a human readable name for the Vendor. The name should be suitable for display in management screens within the Tool Provider system.
        /// </summary>
        [DataMember(Name = "vendor_name")]
        public LocalizedName VendorName { get; private set; }

        /// <summary>
        /// This is the URL of the vendor.
        /// </summary>
        [DataMember(Name = "website")]
        public string Website { get; set; }
    }
}
