using System.Runtime.Serialization;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class Link
    {
        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "rel")]
        public string Rel { get; set; }
    }
}
