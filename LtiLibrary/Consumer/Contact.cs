using System.Runtime.Serialization;

namespace LtiLibrary.Consumer
{
    /// <summary>
    /// Contact information for the associated object.
    /// </summary>
    [DataContract]
    public class Contact
    {
        public Contact(string email)
        {
            Email = email;
        }

        /// <summary>
        /// The email of the primary contact for the associated object.
        /// </summary>
        [DataMember(Name = "email")]
        public string Email { get; private set; }
    }
}
