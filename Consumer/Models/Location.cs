using System.ComponentModel.DataAnnotations;

namespace Consumer.Models
{
    public class District
    {
        public District()
        {
            DistrictId = string.Empty;
            Name = string.Empty;
            StateDistrictId = string.Empty;
            StateId = string.Empty;
        }

        [Key]
        [StringLength(8)]
        public string DistrictId { get; set; }
        public string Name { get; set; }
        public string StateDistrictId { get; set; }
        [StringLength(2)]
        public string StateId { get; set; }
    }

    public class School
    {
        [Key]
        [StringLength(12)]
        public string SchoolId { get; set; }
        public string Name { get; set; }
        [StringLength(8)]
        public string DistrictId { get; set; }
        public string DistrictSchoolId { get; set; }
    }

    public class State
    {
        [Key]
        [StringLength(2)]
        public string StateId { get; set; }
        public string Name { get; set; }
    }
}