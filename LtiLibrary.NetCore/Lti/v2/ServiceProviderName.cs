namespace LtiLibrary.NetCore.Lti.v2
{
    /// <summary>
    /// Represents an IMS ServiceProviderName object.
    /// </summary>
    public class ServiceProviderName : LocalizedName
    {
        /// <summary>
        /// Initializes a new instance of the ServiceProviderName class with a specified name.
        /// </summary>
        public ServiceProviderName(string name)
        {
            Key = "service_provider.name";
            Value = name;
        }
    }
}
