namespace LtiLibrary.NetCore.Lti2
{
    public class ProductDescription : LocalizedText
    {
        public ProductDescription(string description)
        {
            Key = "product.description";
            Value = description;
        }
    }
}
