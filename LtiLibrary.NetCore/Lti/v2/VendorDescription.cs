namespace LtiLibrary.NetCore.Lti.v2
{
    /// <summary>
    /// Represents an IMS VenderDescription object.
    /// </summary>
    public class VendorDescription : LocalizedText
    {
        /// <summary>
        /// Initializes a new instance of the VendorDescription class with a specified description.
        /// </summary>
        public VendorDescription(string description)
        {
            Key = "product.vendor.description";
            Value = description;
        }
    }
}
