using Newtonsoft.Json;

namespace LtiLibrary.Core.Lti2
{
    /// <summary>
    /// Contact information for the associated object.
    /// </summary>
    public class Contact
    {
        public Contact(string email)
        {
            Email = email;
        }

        /// <summary>
        /// The email of the primary contact for the associated object.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; private set; }
    }
}
