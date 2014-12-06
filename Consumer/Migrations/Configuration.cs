using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Consumer.Migrations
{
    using Models;
    using System.Data.Entity.Migrations;

    public sealed class Configuration : DbMigrationsConfiguration<ConsumerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ConsumerContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists(UserRoles.AdminRole)) roleManager.Create(new IdentityRole(UserRoles.AdminRole));
            if (!roleManager.RoleExists(UserRoles.StudentRole)) roleManager.Create(new IdentityRole(UserRoles.StudentRole));
            if (!roleManager.RoleExists(UserRoles.SuperUserRole)) roleManager.Create(new IdentityRole(UserRoles.SuperUserRole));
            if (!roleManager.RoleExists(UserRoles.TeacherRole)) roleManager.Create(new IdentityRole(UserRoles.TeacherRole));

            var teacher = userManager.FindByName("sjohnson@consumer.azurewebsites.net");
            if (teacher == null)
            {
                teacher = new ApplicationUser
                {
                    UserName = "sjohnson@consumer.azurewebsites.net", 
                    FirstName = "Stuart", 
                    LastName = "Johnson",
                    Email = "sjohnson@consumer.azurewebsites.net",
                    SendEmail = true,
                    SendName = true
                };
                userManager.Create(teacher, "Password1!");
                userManager.AddToRole(teacher.Id, UserRoles.TeacherRole);
            }

            context.Courses.AddOrUpdate(new[] {
                new Course {
                    CourseId = 1,
                    Instructor = teacher,
                    Name = "Samples"
                }
            });

            context.Assignments.AddOrUpdate(new[] {
                new Assignment {
                    AssignmentId = 1,
                    ConsumerKey = "12345",
                    ConsumerSecret = "secret",
                    Course = context.Courses.Find(1),
                    CustomParameters = 
@"username=$User.username
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
@"username=$User.username
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
@"username=$User.username
tc_profile_url=$ToolConsumerProfile.url",
                    Description = "Sample LTI Tool Provider.",
                    Name = "Gettysburg Address",
                    Url = "http://localhost:64495/Tool/2"
                }
            });

            context.ContentItemTools.AddOrUpdate(new[]
            {
                new ContentItemTool
                {
                    ContentItemToolId = 1,
                    ConsumerKey = "0e87e688459b45e4",
                    ConsumerSecret = "2aac4af7a1ac4ae7",
                    CustomParameters =
@"username=$User.username",
                    Description = "Find assignments in the Sample Provider Library.",
                    Name = "Sample Provider Library",
                    Owner = teacher,
                    Url = "http://localhost:64495/Tool/Search"
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
tc_profile_url=$ToolConsumerProfile.url",
                    Description = "Sample LTI Tool Provider.",
                    Name = "Gettysburg Address",
                    Url = "http://provider.azurewebsites.net/Tool/2"
                }
            });

            context.ContentItemTools.AddOrUpdate(new[]
            {
                new ContentItemTool
                {
                    ContentItemToolId = 1,
                    ConsumerKey = "1e87e688459b45e4",
                    ConsumerSecret = "2aac4af7a1ac4ae7",
                    CustomParameters =
@"username=$User.username",
                    Description = "Find assignments in the Sample Provider Library.",
                    Name = "Sample Provider Library",
                    Owner = teacher,
                    Url = "http://provider.azurewebsites.net/Tool/Search"
                }
            });
#endif
        }
    }
}
