using System;
using LtiLibrary.NetCore.Common;
using Newtonsoft.Json;

namespace LtiLibrary.NetCore.Lis.v2
{
    /// <summary>
    /// Represents an IMS LISPerson object.
    /// </summary>
    public class Person : JsonLdObject
    {
        /// <summary>
        /// Initialize a new instance of the Person class.
        /// </summary>
        public Person()
        {
            Type = LtiConstants.LisPersonType;
        }

        /// <summary>
        /// The primary email address for the person.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// The person's assigned family name.
        /// </summary>
        [JsonProperty("familyName")]
        public string FamilyName { get; set; }

        /// <summary>
        /// The person's assigned first name.
        /// </summary>
        [JsonProperty("givenName")]
        public string GivenName { get; set; }

        /// <summary>
        /// A URL to an image for the person.
        /// </summary>
        [JsonProperty("image")]
        public Uri Image { get; set; }

        /// <summary>
        /// The person's assigned full name (typically their first name followed by their family name separated with a space).
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// A unique identifier for the person as provisioned by an external system such as an SIS.
        /// </summary>
        [JsonProperty("sourcedId")]
        public string SourcedId { get; set; }

        /// <summary>
        /// A unique identifier for the person.
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}
