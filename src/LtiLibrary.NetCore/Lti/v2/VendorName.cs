namespace LtiLibrary.NetCore.Lti.v2
{
    /// <summary>
    /// Represents an IMS VendorName object.
    /// </summary>
    public class VendorName : LocalizedName
    {
        /// <summary>
        /// Initializes a new instance of the VendorName class with a specified name.
        /// </summary>
        public VendorName(string name)
        {
            Key = "product.vendor.name";
            Value = name;
        }
    }
}
