using System.Runtime.Serialization;

namespace inBloomLibrary.Models
{
    [DataContract]
    public class SessionCheck
    {
        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "external_id")]
        public string ExternalId { get; set; }

        [DataMember(Name = "sliRoles")]
        public string[] Roles { get; set; }

        [DataMember(Name = "tenantId")]
        public string TenantId { get; set; }

        [DataMember(Name = "user_id")]
        public string UserId { get; set; }
    }
}
