using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Provider.Lti;

namespace Provider.Models
{
    public class ProviderContext : IdentityDbContext<ApplicationUser>
    {
        public ProviderContext() : base("DefaultConnection", throwIfV1Schema: false) {}

        public static ProviderContext Create()
        {
            return new ProviderContext();
        }

        public DbSet<PairedUser> PairedUsers { get; set; }
        public DbSet<Tool> Tools { get; set; }

        // From LtiLibrary
        public DbSet<Consumer> Consumers { get; set; }
        public DbSet<ProviderRequest> ProviderRequests { get; set; }
        public DbSet<Outcome> Outcomes { get; set; }
    }
}