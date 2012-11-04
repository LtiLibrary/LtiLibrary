using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Consumer.Models
{
    public class SharingScope
    {
        [Key]
        public int SharingScopeId { get; set; }
        public string Name { get; set; }

        public const int Private = 1;
        public const int School = 2;
        public const int District = 3;
        public const int State = 4;
        public const int Public = 5;
    }

    public class LtiVersion
    {
        [Key]
        public int LtiVersionId { get; set; }
        public string Name { get; set; }

        public const int Version10 = 1;
        public const int Version11 = 2;
    }

    public class Assignment
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AssignmentId { get; set; }
        [Display(Name = "Key")]
        public string ConsumerKey { get; set; }
        [Display(Name = "Custom Parameters")]
        public string CustomParameters { get; set; }
        public string Description { get; set; }
        [Display(Name="LTI Version"), ForeignKey("LtiVersion"), Required]
        public int LtiVersionId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Secret { get; set; }
        [Display(Name="Sharing Scope"), ForeignKey("SharingScope")]
        public int SharingScopeId { get; set; }
        [DataType(DataType.Url), Required]
        public string Url { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual LtiVersion LtiVersion { get; set; }
        public virtual SharingScope SharingScope { get; set; }
        public virtual User User { get; set; }

        [NotMapped, Display(Name = "LTI Link")]
        public bool IsLtiLink
        {
            get
            {
                return !string.IsNullOrEmpty(ConsumerKey) && !string.IsNullOrEmpty(Secret);
            }
        }
    }
}