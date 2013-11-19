using System.Runtime.Serialization;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class CourseOffering
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "localCourseCode")]
        public string LocalCourseCode { get; set; }

        [DataMember(Name = "localCourseTitle")]
        public string LocalCourseTitle { get; set; }
    }
}
