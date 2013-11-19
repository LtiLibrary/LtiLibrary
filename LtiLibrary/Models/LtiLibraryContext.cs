using System.Data.Entity;

namespace LtiLibrary.Models
{
    public class LtiLibraryContext : DbContext
    {
        public LtiLibraryContext() : base("DefaultConnection")
        {
            // This allows another project to use this library, even if
            // the other project is using EntityFramework with migrations
            // enabled.
            Database.SetInitializer<LtiLibraryContext>(null);
        }

        public DbSet<Consumer> Consumers { get; set; }
        public DbSet<LtiInboundRequest> LtiInboundRequests { get; set; }
        public DbSet<Outcome> Outcomes { get; set; }
    }
}
