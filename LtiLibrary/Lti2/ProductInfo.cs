using System.Runtime.Serialization;

namespace LtiLibrary.Lti2
{
    /// <summary>
    /// Encapsulates information about one released version of a product (Tool Consumer or Tool).
    /// </summary>
    [DataContract]
    public class ProductInfo
    {
        public ProductInfo(ProductFamily productFamily, string productName, string productVersion)
        {
            // Required
            ProductFamily = productFamily;
            ProductName = new LocalizedName {Key = "product.name", Value = productName};
            ProductVersion = productVersion;
        }

        /// <summary>
        /// This is a description of the product suitable for display to end-users.
        /// </summary>
        [DataMember(Name = "description")]
        public LocalizedName Description { get; set; }

        /// <summary>
        /// An inverse attribute that references the ProductFamily within which this ProductInfo is defined.
        /// </summary>
        [DataMember(Name = "product_family")]
        public ProductFamily ProductFamily { get; private set; }

        /// <summary>
        /// A name for the product suitable for display to end users.
        /// </summary>
        [DataMember(Name = "product_name")]
        public LocalizedName ProductName { get; private set; }

        /// <summary>
        /// This is the version of the product. Should match tool_consumer_info_version launch parameter.
        /// </summary>
        [DataMember(Name = "product_version")]
        public string ProductVersion { get; private set; }

        /// <summary>
        /// This is a human readable description of the technical aspects of this product that might 
        /// be of interest to developers who wish to integrate with this product via LTI.
        /// </summary>
        [DataMember(Name = "technical_description")]
        public LocalizedName TechnicalDescription { get; set; }
    }
}
