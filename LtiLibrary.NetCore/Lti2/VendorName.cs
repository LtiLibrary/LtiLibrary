namespace LtiLibrary.NetCore.Lti2
{
    public class VendorName : LocalizedName
    {
        public VendorName(string name)
        {
            Key = "product.vendor.name";
            Value = name;
        }
    }
}
