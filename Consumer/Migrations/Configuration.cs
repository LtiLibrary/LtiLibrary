namespace Consumer.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Consumer.Models.ConsumerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Consumer.Models.ConsumerContext context)
        {
            context.SharingScopes.AddOrUpdate(
                new Consumer.Models.SharingScope { SharingScopeId = Consumer.Models.SharingScope.Private, Name = "Private" },
                new Consumer.Models.SharingScope { SharingScopeId = Consumer.Models.SharingScope.School, Name = "School" },
                new Consumer.Models.SharingScope { SharingScopeId = Consumer.Models.SharingScope.District, Name = "District" },
                new Consumer.Models.SharingScope { SharingScopeId = Consumer.Models.SharingScope.State, Name = "State" },
                new Consumer.Models.SharingScope { SharingScopeId = Consumer.Models.SharingScope.Public, Name = "Public" }
                );

            context.LtiVersions.AddOrUpdate(
                new Consumer.Models.LtiVersion { LtiVersionId = Consumer.Models.LtiVersion.Version10, Name = "LTI 1.0" },
                new Consumer.Models.LtiVersion { LtiVersionId = Consumer.Models.LtiVersion.Version11, Name = "LTI 1.1" }
                );

            context.Configuration.AutoDetectChangesEnabled = false;
            (context as IObjectContextAdapter).ObjectContext.CommandTimeout = 0;

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("Consumer.App_Data.States.sql"))
            using (var reader = new StreamReader(stream))
            {
                context.Database.ExecuteSqlCommand(reader.ReadToEnd(), new object[] { });
            }
            using (var stream = assembly.GetManifestResourceStream("Consumer.App_Data.Districts.sql"))
            using (var reader = new StreamReader(stream))
            {
                context.Database.ExecuteSqlCommand(reader.ReadToEnd(), new object[] { });
            }
            using (var stream = assembly.GetManifestResourceStream("Consumer.App_Data.Schools.sql"))
            using (var reader = new StreamReader(stream))
            {
                context.Database.ExecuteSqlCommand(reader.ReadToEnd(), new object[] { });
            }
            context.Configuration.AutoDetectChangesEnabled = true;
            (context as IObjectContextAdapter).ObjectContext.CommandTimeout = 30;
        }
    }
}
