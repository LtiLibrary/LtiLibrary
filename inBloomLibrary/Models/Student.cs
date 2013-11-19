using System.Runtime.Serialization;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class Student
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "studentUniqueStateId")]
        public string StateId { get; set; }

        [DataMember(Name = "name")]
        public Name Name { get; set; }

        [DataMember(Name = "electronicMail")]
        public ElectronicMail[] Email { get; set; }
    }
}
