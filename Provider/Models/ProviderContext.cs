using System.Data.Entity;
using LtiLibrary.Models;

namespace Provider.Models
{
    public class ProviderContext : DbContext
    {
        public ProviderContext() : base("DefaultConnection") {}

        public DbSet<District> Districts { get; set; }
        public DbSet<PairedUser> PairedUsers { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Tool> Tools { get; set; }
        public DbSet<User> Users { get; set; }

        // From LtiLibrary
        public DbSet<Consumer> Consumers { get; set; }
        public DbSet<LtiInboundRequest> LtiRequests { get; set; }
        public DbSet<Outcome> Outcomes { get; set; }
    }
}