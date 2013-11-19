using System.Data.Entity;

namespace inBloomLibrary.Models
{
    public class inBloomLibraryContext : DbContext
    {
        public inBloomLibraryContext()
            : base("DefaultConnection")
        {
            // This allows another project to use this library, even if
            // the other project is using EntityFramework with migrations
            // enabled.
            Database.SetInitializer<inBloomLibraryContext>(null);
        }

        public DbSet<Tenant> Tenants { get; set; }
    }
}
