using System.Data.Entity;

namespace Consumer.Models
{
    public class ConsumerContext : DbContext
    {
        public ConsumerContext() : base("DefaultConnection") {}

        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<OAuthLibrary.Models.AccessToken> OAuthAccessTokens { get; set; }
        public DbSet<inBloomLibrary.Models.Tenant> Tenants { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasMany(x => x.EnrolledUsers)
                .WithMany(x => x.Courses)
                .Map(x =>
                    {
                        x.ToTable("CourseEnrolledUsers");
                        x.MapLeftKey("CourseId");
                        x.MapRightKey("EnrolledUserId");
                    });
            base.OnModelCreating(modelBuilder);
        }
    }
}
