namespace LtiLibrary.NetCore.Lti2
{
    /// <summary>
    /// Represents an IMS ServiceOwnerName object.
    /// </summary>
    public class ServiceOwnerName : LocalizedName
    {
        /// <summary>
        /// Initializes a new instance of the ServiceOwnerName class with a specified name.
        /// </summary>
        public ServiceOwnerName(string name)
        {
            Key = "service_owner.name";
            Value = name;
        }
    }
}
