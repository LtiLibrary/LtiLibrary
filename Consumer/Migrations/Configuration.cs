namespace Consumer.Migrations
{
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using Models;

    public sealed class Configuration : DbMigrationsConfiguration<ConsumerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ConsumerContext context)
        {
            context.Configuration.AutoDetectChangesEnabled = false;
            (context as IObjectContextAdapter).ObjectContext.CommandTimeout = 0;

            // Tried embedding the sql, but the resulting App.dll was huge, which meant
            // that publishing was kind of slow. Changed to treating the sql as content
            // and uploading once.
            var appData = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
            if (context.Database.SqlQuery<int>("select count(0) from States").FirstOrDefault() == 0)
            {
                using (var archive = ZipFile.OpenRead(Path.Combine(appData, "States.zip")))
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
                using (var archive = ZipFile.OpenRead(Path.Combine(appData, "Districts.zip")))
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
                using (var archive = ZipFile.OpenRead(Path.Combine(appData, "Schools.zip")))
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

            context.Courses.AddOrUpdate(new [] {
                new Course {
                    CourseId = 1,
                    Instructor = context.Users.Find(1),
                    Name = "Samples",
                    State = context.States.Find("OR"),
                    District = context.Districts.Find("4101920"),
                    School = context.Schools.Find("410192000554"),
                }
            });

            context.Assignments.AddOrUpdate(new[] {
                new Assignment {
                    AssignmentId = 1,
                    ConsumerKey = "12345",
                    ConsumerSecret = "secret",
                    Course = context.Courses.Find(1),
                    CustomParameters = 
@"custom_username=$User.username
custom_state_id=$Context.stateId
custom_district_id=$Context.ncesLeaId
custom_school_id=$Context.ncesSchoolId
tc_profile_url=$ToolConsumerProfile.url",
                    Description = "IMS Global LTI developer test tool echos back the values.",
                    Name = "LTI Test Tool",
                    Url = "http://www.imsglobal.org/developers/LTI/test/v1p1/tool.php"
                }
            });
#if DEBUG
            context.Assignments.AddOrUpdate(new[] {
                new Assignment {
                    AssignmentId = 2,
                    ConsumerKey = "0e87e688459b45e4",
                    ConsumerSecret = "2aac4af7a1ac4ae7",
                    Course = context.Courses.Find(1),
                    CustomParameters = 
@"custom_username=$User.username
custom_state_id=$Context.stateId
custom_district_id=$Context.ncesLeaId
custom_school_id=$Context.ncesSchoolId
tc_profile_url=$ToolConsumerProfile.url",
                    Description = "Sample LTI Tool Provider.",
                    Name = "Emancipation Proclamation",
                    Url = "http://localhost:64495/Tool/1"
                },
                new Assignment {
                    AssignmentId = 3,
                    ConsumerKey = "0e87e688459b45e4",
                    ConsumerSecret = "2aac4af7a1ac4ae7",
                    Course = context.Courses.Find(1),
                    CustomParameters = 
@"custom_username=$User.username
custom_state_id=$Context.stateId
custom_district_id=$Context.ncesLeaId
custom_school_id=$Context.ncesSchoolId
tc_profile_url=$ToolConsumerProfile.url",
                    Description = "Sample LTI Tool Provider.",
                    Name = "Gettysburg Address",
                    Url = "http://localhost:64495/Tool/2"
                }
            });
            context.Tenants.AddOrUpdate(new[] {
                new inBloomLibrary.Models.Tenant {
                    TenantId = 1,
                    ClientId = "EkG5ov854Q",
                    LongLivedSessionToken = "t-a3e3912b-12c7-4e34-830d-94075a14d7de",
                    SharedSecret = "lOBKVRMVaTGmhWyqY0jmDLYi9nUVCxBAMQf13TDCBaos9V3z",
                    inBloomTenantId = "aamiller@alumni.princeton.edu",
                    UserId = 1
                }
            });
#else
            context.Assignments.AddOrUpdate(new[] {
                new Assignment {
                    AssignmentId = 2,
                    ConsumerKey = "1e87e688459b45e4",
                    ConsumerSecret = "2aac4af7a1ac4ae7",
                    Course = context.Courses.Find(1),
                    CustomParameters = 
@"username=$User.username
state_id=$Context.stateId
district_id=$Context.ncesLeaId
school_id=$Context.ncesSchoolId
tc_profile_url=$ToolConsumerProfile.url",
                    Description = "Sample LTI Tool Provider.",
                    Name = "Emancipation Proclamation",
                    Url = "http://provider.azurewebsites.net/Tool/1"
                },
                new Assignment {
                    AssignmentId = 3,
                    ConsumerKey = "1e87e688459b45e4",
                    ConsumerSecret = "2aac4af7a1ac4ae7",
                    Course = context.Courses.Find(1),
                    CustomParameters = 
@"username=$User.username
state_id=$Context.stateId
district_id=$Context.ncesLeaId
school_id=$Context.ncesSchoolId
tc_profile_url=$ToolConsumerProfile.url",
                    Description = "Sample LTI Tool Provider.",
                    Name = "Gettysburg Address",
                    Url = "http://provider.azurewebsites.net/Tool/2"
                }
            });
            context.Tenants.AddOrUpdate(new[] {
                new inBloomLibrary.Models.Tenant {
                    TenantId = 1,
                    ClientId = "sWSp0VRHJj",
                    LongLivedSessionToken = "t-033c7652-8ae0-4da8-a305-26cc6a8f44f4",
                    SharedSecret = "viQxlnUyRw0UQ8mxApmEmHXk2yGpgMQpZ8vxIQOVFPmg6XfL",
                    inBloomTenantId = "aamiller@alumni.princeton.edu",
                    UserId = 1
                }
            });
#endif
        }
    }
}
