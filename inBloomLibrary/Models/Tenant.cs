using System.ComponentModel.DataAnnotations;

namespace inBloomLibrary.Models
{
    public class Tenant
    {
        public int TenantId { get; set; }

        [Display(Name = "Client ID")]
        public string ClientId { get; set; }

        [Display(Name = "API Token")]
        public string LongLivedSessionToken { get; set; }

        [Display(Name = "Shared Secret")]
        public string SharedSecret { get; set; }

        [Required]
        [MaxLength(256)]
        [Display(Name = "Tenant ID")]
        public string inBloomTenantId { get; set; }

        public int UserId { get; set; }
    }
}