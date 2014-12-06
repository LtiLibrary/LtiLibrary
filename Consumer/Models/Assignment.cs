using System.ComponentModel.DataAnnotations;

namespace Consumer.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
        
        [Display(Name = "Key")]
        public string ConsumerKey { get; set; }
        
        [Display(Name = "Secret")]
        public string ConsumerSecret { get; set; }
        
        public virtual Course Course { get; set; }
        
        [Display(Name = "Custom Parameters")]
        public string CustomParameters { get; set; }
        
        public string Description { get; set; }
        
        public bool IsLtiLink
        {
            get
            {
                return !string.IsNullOrEmpty(ConsumerKey) && !string.IsNullOrEmpty(ConsumerSecret);
            }
        }
        
        public string Name { get; set; }

        [Display(Name = "URL")]
        public string Url { get; set; }
    }

    public class CreateEditAssignmentModel
    {
        public int AssignmentId { get; set; }

        [Display(Name = "Key")]
        public string ConsumerKey { get; set; }

        [Display(Name = "Secret")]
        public string ConsumerSecret { get; set; }

        public int CourseId { get; set; }

        [Display(Name = "Custom Parameters")]
        public string CustomParameters { get; set; }

        public string Description { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        public CreateEditAssignmentModel() { }
        public CreateEditAssignmentModel(Assignment assignment)
        {
            AssignmentId = assignment.AssignmentId;
            ConsumerKey = assignment.ConsumerKey;
            ConsumerSecret = assignment.ConsumerSecret;
            CourseId = assignment.Course.CourseId;
            CustomParameters = assignment.CustomParameters;
            Description = assignment.Description;
            Name = assignment.Name;
            Url = assignment.Url;
        }
    }

    public class DeleteAssignmentModel
    {
        public int AssignmentId { get; set; }

        [Display(Name = "Key")]
        public string ConsumerKey { get; set; }

        [Display(Name = "Secret")]
        public string ConsumerSecret { get; set; }

        public int CourseId { get; set; }

        [Display(Name = "Custom Parameters")]
        public string CustomParameters { get; set; }

        public string Description { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        public DeleteAssignmentModel() { }
        public DeleteAssignmentModel(Assignment assignment)
        {
            AssignmentId = assignment.AssignmentId;
            ConsumerKey = assignment.ConsumerKey;
            ConsumerSecret = assignment.ConsumerSecret;
            CourseId = assignment.Course.CourseId;
            CustomParameters = assignment.CustomParameters;
            Description = assignment.Description;
            Name = assignment.Name;
            Url = assignment.Url;
        }
    }

    public class ScoredAssignmentModel
    {
        public ScoredAssignmentModel(Assignment assignment)
        {
            AssignmentId = assignment.AssignmentId;
            Course = assignment.Course;
            Description = assignment.Description;
            Name = assignment.Name;
            Url = assignment.Url;
        }

        public int AssignmentId { get; set; }
        public string Description { get; set; }
        public Course Course { get; set; }
        public string Name { get; set; }
        public string Score { get; set; }
        public string Url { get; set; }
        public string UserId { get; set; }
    }

    public class LaunchModel
    {
        public int AssignmentId { get; set; }
        public string AssignmentTitle { get; set; }
        public string CourseTitle { get; set; }
        public bool IsLtiLink { get; set; }
        public string ReturnUrl { get; set; }
        public string Url { get; set; }
    }
}