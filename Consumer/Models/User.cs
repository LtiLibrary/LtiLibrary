using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace Consumer.Models
{
    public static class UserRoles
    {
        public static string StudentRole = "Student";
        public static string TeacherRole = "Teacher";
    }

    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }

        [ForeignKey("District")]
        public string DistrictId { get; set; }
        [Display(Name = "Email address"), DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [ForeignKey("School")]
        public string SchoolId { get; set; }
        [Display(Name = "Send my email address to providers when I launch an assignment")]
        public bool? SendEmail { get; set; }
        [Display(Name = "Send my name to providers when I launch an assignment")]
        public bool? SendName { get; set; }
        [ForeignKey("State")]
        public string StateId { get; set; }

        public virtual District District { get; set; }
        public virtual School School { get; set; }
        public virtual State State { get; set; }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}",
                    FirstName,
                    LastName
                ).Trim();
            }
        }

        // Move to a derived model?
        [NotMapped, Display(Name = "Student")]
        public bool IsStudent { get; set; }
        [NotMapped, Display(Name = "Teacher")]
        public bool IsTeacher { get; set; }
    }

    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }
}