using System.Runtime.Serialization;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class Name
    {
        [DataMember(Name = "lastSurname")]
        public string LastName { get; set; }

        [DataMember(Name = "firstName")]
        public string FirstName { get; set; }
    }
}
