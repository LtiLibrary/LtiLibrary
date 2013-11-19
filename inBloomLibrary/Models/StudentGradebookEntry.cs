using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class StudentGradebookEntry
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "dateFulfilled")]
        private string _dateFulfilled;
        public DateTime DateFulfilled
        {
            get
            {
                return DateTime.ParseExact(_dateFulfilled, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            set
            {
                _dateFulfilled = value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
        }

        [DataMember(Name = "gradebookEntryId")]
        public string GradebookEntryId { get; set; }

        [DataMember(Name = "numericGradeEarned")]
        public int NumericGradeEarned { get; set; }

        [DataMember(Name = "sectionId")]
        public string SectionId { get; set; }

        [DataMember(Name = "studentId")]
        public string StudentId { get; set; }

        [DataMember(Name = "studentSectionAssociationId")]
        public string StudentSectionAssociationId { get; set; }
    }
}
