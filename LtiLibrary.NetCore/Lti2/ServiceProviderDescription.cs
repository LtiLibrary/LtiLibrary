namespace LtiLibrary.NetCore.Lti2
{
    /// <summary>
    /// Represents an IMS ServiceProviderDescription object.
    /// </summary>
    public class ServiceProviderDescription : LocalizedText
    {
        /// <summary>
        /// Initializes a new instance of the ServiceProviderDescription class with a specified description.
        /// </summary>
        public ServiceProviderDescription(string description)
        {
            Key = "service_provider.description";
            Value = description;
        }
    }
}
