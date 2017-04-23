namespace LtiLibrary.NetCore.Lti2
{
    /// <summary>
    /// Represents an IMS ProductTechnicalDescription object.
    /// </summary>
    public class ProductTechnicalDescription : LocalizedText
    {
        /// <summary>
        /// Initializes a new instance of the ProductTechnicalDescription class with a specified description.
        /// </summary>
        public ProductTechnicalDescription(string description)
        {
            Key = "product.technicalDescription";
            Value = description;
        }
    }
}
