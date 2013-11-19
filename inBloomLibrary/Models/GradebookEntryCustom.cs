using System.Runtime.Serialization;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class GradebookEntryCustom
    {
        [DataMember(Name = "consumerKey")]
        public string ConsumerKey { get; set; }

        [DataMember(Name = "consumerSecret")]
        public string ConsumerSecret { get; set; }

        [DataMember(Name = "customParameters")]
        public string CustomParameters { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}
