using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Consumer.Models
{
    public class ConsumerContext : IdentityDbContext<ApplicationUser>
    {
        public ConsumerContext() : base("DefaultConnection", throwIfV1Schema: false) { }

        public static ConsumerContext Create()
        {
            return new ConsumerContext();
        }

        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<ContentItemTool> ContentItemTools { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Score> Scores { get; set; }
    }
}
