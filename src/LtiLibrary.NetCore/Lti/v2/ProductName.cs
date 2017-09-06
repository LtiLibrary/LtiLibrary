namespace LtiLibrary.NetCore.Lti.v2
{
    /// <summary>
    /// Represents an IMS ProductName.
    /// </summary>
    public class ProductName : LocalizedName
    {
        /// <summary>
        /// Initializes a new instance of the ProductName with the specificed name.
        /// </summary>
        /// <param name="name">The product name.</param>
        public ProductName(string name)
        {
            Key = "product.name";
            Value = name;
        }
    }
}
