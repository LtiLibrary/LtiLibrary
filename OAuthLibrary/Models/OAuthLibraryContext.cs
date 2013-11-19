using System.Data.Entity;

namespace OAuthLibrary.Models
{
    public class OAuthLibraryContext : DbContext
    {
        public OAuthLibraryContext()
            : base("DefaultConnection")
        {
            // This allows another project to use this library, even if
            // the other project is using EntityFramework with migrations
            // enabled.
            Database.SetInitializer<OAuthLibraryContext>(null);
        }

        public DbSet<AccessToken> AccessTokens { get; set; }    
    }
}
