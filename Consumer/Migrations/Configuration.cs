namespace Consumer.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<Consumer.Models.ConsumerContext>
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
            // Tried embedding the sql, but the resulting App.dll was huge, which meant
            // the publishing was kind of slow. Changed to treating the sql as content
            // and uploading once.
            var app_data = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            if (context.Database.SqlQuery<int>("select count(0) from States").FirstOrDefault() == 0)
            {
                using (var archive = ZipFile.OpenRead(Path.Combine(app_data, "States.zip")))
                {
                    var entry = archive.GetEntry("States.sql");
                    using (var reader = new StreamReader(entry.Open()))
                    {
                        context.Database.ExecuteSqlCommand(reader.ReadToEnd(), new object[] { });
                    }
                }
            }
            if (context.Database.SqlQuery<int>("select count(0) from Districts").FirstOrDefault() == 0)
            {
                using (var archive = ZipFile.OpenRead(Path.Combine(app_data, "Districts.zip")))
                {
                    var entry = archive.GetEntry("Districts.sql");
                    using (var reader = new StreamReader(entry.Open()))
                    {
                        context.Database.ExecuteSqlCommand(reader.ReadToEnd(), new object[] { });
                    }
                }
            }
            if (context.Database.SqlQuery<int>("select count(0) from Schools").FirstOrDefault() == 0)
            {
                using (var archive = ZipFile.OpenRead(Path.Combine(app_data, "Schools.zip")))
                {
                    var entry = archive.GetEntry("Schools.sql");
                    using (var reader = new StreamReader(entry.Open()))
                    {
                        context.Database.ExecuteSqlCommand(reader.ReadToEnd(), new object[] { });
                    }
                }
            }
            context.Configuration.AutoDetectChangesEnabled = true;
            (context as IObjectContextAdapter).ObjectContext.CommandTimeout = 30;
        }
    }
}
