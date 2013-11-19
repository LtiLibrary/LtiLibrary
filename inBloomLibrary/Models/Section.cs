using System.Runtime.Serialization;
using System.Collections.Generic;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class Section
    {
        [DataMember(Name = "courseOfferingId")]
        public string CourseOfferingId { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "links")]
        public Link[] Links { get; set; }

        public IEnumerable<GradebookEntry> GradebookEntries { get; set; }

        [DataMember(Name = "schoolId")]
        public string SchoolId { get; set; }

        public IEnumerable<Student> Students { get; set; }

        [DataMember(Name = "uniqueSectionCode")]
        public string UniqueSectionCode { get; set; }
    }
}
