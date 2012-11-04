using System.Data.Entity;

namespace Consumer.Models
{
    public class ConsumerContext : DbContext
    {
        public ConsumerContext() : base("DefaultConnection") {}

        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<LtiVersion> LtiVersions { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<SharingScope> SharingScopes { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
