using System.Runtime.Serialization;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class ElectronicMail
    {
        [DataMember(Name = "emailAddressType")]
        public string EmailAddressType { get; set; }

        [DataMember(Name = "emailAddress")]
        public string EmailAddress { get; set; }
    }
}
