using System.ComponentModel.DataAnnotations;

namespace Consumer.Models
{
    public class ContentItemTool
    {
        public int ContentItemToolId { get; set; }

        [Required]
        [Display(Name = "Key")]
        public string ConsumerKey { get; set; }

        [Required]
        [Display(Name = "Secret")]
        public string ConsumerSecret { get; set; }

        [Display(Name = "Custom Parameters")]
        public string CustomParameters { get; set; }

        public string Description { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        [Required]
        [Display(Name = "URL")]
        public string Url { get; set; }
    }

    public class ContentItemToolViewModel
    {
        public int ContentItemToolId { get; set; }
        public int CourseId { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }

    public class ContentItemToolLaunchModel
    {
        public int ContentItemToolId { get; set; }
        public string ContentItemToolName { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string ReturnUrl { get; set; }
    }
}