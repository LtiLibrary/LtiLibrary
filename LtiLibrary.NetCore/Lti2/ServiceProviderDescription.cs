namespace LtiLibrary.NetCore.Lti2
{
    public class ServiceProviderDescription : LocalizedText
    {
        public ServiceProviderDescription(string description)
        {
            Key = "service_provider.description";
            Value = description;
        }
    }
}
