using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lti2
{
    /// <summary>
    /// A ProductFamily represents the collection of all versions of a particular product over time. Two products that 
    /// have the same product code belong to the same ProductFamily. The LTI product model has four levels:
    /// <list type="number">
    /// <item>
    /// <description>
    /// ProductFamily: The collection of all versions of a given product.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// ProductInfo: Encapsulates information about one particular version of a product.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// ProductInstance: Represents one deployed instance of a particular version of a product.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// ProductProfile: Defines a view of a deployed instance of a product. This view may be customized for different integration partners.
    /// </description>
    /// </item>
    /// </list>
    /// Notice that the ProductFamily sits at the top of the product hierarchy. Each product family is owned by a Vendor.
    /// </summary>
    public class ProductFamily
    {
        /// <summary>
        /// Should match tool_consumer_info_product_family_code launch parameter.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// The vendor that owns the ProductFamily.
        /// </summary>
        [JsonProperty("vendor")]
        public Vendor Vendor { get; set; }
    }
}
