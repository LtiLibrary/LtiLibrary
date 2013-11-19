using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class GradebookEntry
    {
        [DataMember(Name = "dateAssigned")]
        private string _dateAssigned;

        public DateTime DateAssigned 
        {
            get
            {
                return DateTime.ParseExact(_dateAssigned, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            set
            {
                _dateAssigned = value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
        }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "gradebookEntryType")]
        public string GradebookEntryType { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "sectionId")]
        public string SectionId { get; set; }

        public GradebookEntryCustom Custom { get; set; }
    }
}
