namespace LtiLibrary.NetCore.Lti2
{
    /// <summary>
    /// Represents an IMS ServiceOwnerDescription object.
    /// </summary>
    public class ServiceOwnerDescription : LocalizedText
    {
        /// <summary>
        /// Initializes a new instance of the ServiceOwnerDescription class with a specified description.
        /// </summary>
        public ServiceOwnerDescription(string description)
        {
            Key = "service_owner.description";
            Value = description;
        }
    }
}
