using System.ComponentModel.DataAnnotations;

namespace Provider.Models
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
            UserName = user.UserName;
        }
    }
}