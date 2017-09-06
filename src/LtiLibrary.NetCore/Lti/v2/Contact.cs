﻿using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lti.v2
{
    /// <summary>
    /// Contact information for the associated object.
    /// </summary>
    public class Contact
    {
        /// <summary>
        /// Initializes a new instance of the Contact class with the specified email address.
        /// </summary>
        /// <param name="email">The email of the primary contact.</param>
        public Contact(string email)
        {
            Email = email;
        }

        /// <summary>
        /// Gets or sets the email of the primary contact for the associated object.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; private set; }
    }
}
