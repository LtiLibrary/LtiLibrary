using System.ComponentModel.DataAnnotations;

namespace Consumer.Models
{
    public class UserProfileModel
    {
        public string UserId { get; set; }

        [Display(Name = "Email address"), DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Student")]
        public bool IsStudent { get; set; }

        [Display(Name = "Teacher")]
        public bool IsTeacher { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Send my email address to providers when I launch an assignment")]
        public bool? SendEmail { get; set; }

        [Display(Name = "Send my name to providers when I launch an assignment")]
        public bool? SendName { get; set; }

        public string UserName { get; set; }

        public UserProfileModel()
        {
        }

        public UserProfileModel(ApplicationUser user)
        {
            UserId = user.Id;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            SendEmail = user.SendEmail;
            SendName = user.SendName;
            UserName = user.UserName;
        }
    }
}