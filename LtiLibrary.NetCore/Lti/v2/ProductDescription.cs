namespace LtiLibrary.NetCore.Lti.v2
{
    /// <summary>
    /// Represents the IMS ProductDescription.
    /// </summary>
    public class ProductDescription : LocalizedText
    {
        /// <summary>
        /// Initializes a new instance of the ProductDescription with the specified description.
        /// </summary>
        /// <param name="description">The product description.</param>
        public ProductDescription(string description)
        {
            Key = "product.description";
            Value = description;
        }
    }
}
