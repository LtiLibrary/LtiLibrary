using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class StudentSectionAssociation
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "sectionId")]
        public string SectionId { get; set; }

        [DataMember(Name = "studentId")]
        public string StudentId { get; set; }

        [DataMember(Name = "beginDate")]
        private string _beginDate;
        public DateTime BeginDate
        {
            get
            {
                return DateTime.ParseExact(_beginDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            set
            {
                _beginDate = value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
        }

        [DataMember(Name = "endDate")]
        private string _endDate;
        public DateTime EndDate
        {
            get
            {
                return DateTime.ParseExact(_endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            set
            {
                _endDate = value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
        }
    }
}
