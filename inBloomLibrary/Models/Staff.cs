using System.Runtime.Serialization;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class Staff
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public Name Name { get; set; }
    }
}
