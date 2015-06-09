namespace LtiLibrary.Core.Lti2
{
    public class VendorDescription : LocalizedText
    {
        public VendorDescription(string description)
        {
            Key = "product.vendor.description";
            Value = description;
        }
    }
}
