using System.Runtime.Serialization;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class Home
    {
        [DataMember(Name = "links")]
        public Link[] Links { get; set; }
    }
}
