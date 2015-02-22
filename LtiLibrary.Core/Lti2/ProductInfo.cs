using Newtonsoft.Json;

namespace LtiLibrary.Core.Lti2
{
    /// <summary>
    /// Encapsulates information about one released version of a product (Tool Consumer or Tool).
    /// </summary>
    public class ProductInfo
    {
        /// <summary>
        /// This is a description of the product suitable for display to end-users.
        /// </summary>
        [JsonProperty("description")]
        public LocalizedName Description { get; set; }

        /// <summary>
        /// An inverse attribute that references the ProductFamily within which this ProductInfo is defined.
        /// </summary>
        [JsonProperty("product_family")]
        public ProductFamily ProductFamily { get; set; }

        /// <summary>
        /// A name for the product suitable for display to end users.
        /// </summary>
        [JsonProperty("product_name")]
        public LocalizedName ProductName { get; set; }

        /// <summary>
        /// This is the version of the product. Should match tool_consumer_info_version launch parameter.
        /// </summary>
        [JsonProperty("product_version")]
        public string ProductVersion { get; set; }

        /// <summary>
        /// This is a human readable description of the technical aspects of this product that might 
        /// be of interest to developers who wish to integrate with this product via LTI.
        /// </summary>
        [JsonProperty("technical_description")]
        public LocalizedName TechnicalDescription { get; set; }
    }
}
