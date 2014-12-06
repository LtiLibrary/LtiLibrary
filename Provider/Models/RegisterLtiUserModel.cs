using System.ComponentModel.DataAnnotations;

namespace Provider.Models
{
    public class RegisterLtiUserModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string ConsumerName { get; set; }
        public int LtiRequestId { get; set; }
    }
}