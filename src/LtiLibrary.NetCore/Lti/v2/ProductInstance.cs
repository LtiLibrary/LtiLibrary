using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lti.v2
{
    /// <summary>
    /// The resource encapsulates information about one deployed instance of a web-based product (Tool or Tool Consumer).
    /// </summary>
    public class ProductInstance
    {
        /// <summary>
        /// A globally unique identifier for the service provider. As a best practice, this value should match an 
        /// Internet domain name assigned by ICANN, but any globally unique identifier is acceptable. 
        /// Should match the tool_consumer_instance_guid launch parameter.
        /// </summary>
        [JsonProperty("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// This is metadata about the product described in the profile.
        /// </summary>
        [JsonProperty("product_info")]
        public ProductInfo ProductInfo { get; set; }

        /// <summary>
        /// The entity that owns the services provided by this product instance.
        /// </summary>
        [JsonProperty("service_owner")]
        public ServiceOwner ServiceOwner { get; set; }

        /// <summary>
        /// This is the service provider that hosts the deployed product.
        /// </summary>
        [JsonProperty("service_provider")]
        public ServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Contact information for support on this deployed instance of the product (Tool or Tool Consumer).
        /// </summary>
        [JsonProperty("support")]
        public Contact Support { get; set; }
    }
}
