namespace Consumer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Schema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Assignments",
                c => new
                    {
                        AssignmentId = c.Int(nullable: false, identity: true),
                        ConsumerKey = c.String(),
                        ConsumerSecret = c.String(),
                        CustomParameters = c.String(),
                        Description = c.String(),
                        Name = c.String(),
                        inBloomGradebookEntryId = c.String(),
                        inBloomTenantId = c.String(),
                        Url = c.String(),
                        Course_CourseId = c.Int(),
                    })
                .PrimaryKey(t => t.AssignmentId)
                .ForeignKey("dbo.Courses", t => t.Course_CourseId)
                .Index(t => t.Course_CourseId);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        CourseId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        inBloomSectionId = c.String(),
                        Instructor_UserId = c.Int(),
                        State_StateId = c.String(maxLength: 2),
                        District_DistrictId = c.String(maxLength: 8),
                        School_SchoolId = c.String(maxLength: 12),
                    })
                .PrimaryKey(t => t.CourseId)
                .ForeignKey("dbo.Users", t => t.Instructor_UserId)
                .ForeignKey("dbo.States", t => t.State_StateId)
                .ForeignKey("dbo.Districts", t => t.District_DistrictId)
                .ForeignKey("dbo.Schools", t => t.School_SchoolId)
                .Index(t => t.Instructor_UserId)
                .Index(t => t.State_StateId)
                .Index(t => t.District_DistrictId)
                .Index(t => t.School_SchoolId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        SendEmail = c.Boolean(),
                        SendName = c.Boolean(),
                        SlcUserId = c.String(),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.States",
                c => new
                    {
                        StateId = c.String(nullable: false, maxLength: 2),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.StateId);
            
            CreateTable(
                "dbo.Districts",
                c => new
                    {
                        DistrictId = c.String(nullable: false, maxLength: 8),
                        Name = c.String(),
                        StateDistrictId = c.String(),
                        StateId = c.String(maxLength: 2),
                    })
                .PrimaryKey(t => t.DistrictId);
            
            CreateTable(
                "dbo.Schools",
                c => new
                    {
                        SchoolId = c.String(nullable: false, maxLength: 12),
                        Name = c.String(),
                        DistrictId = c.String(maxLength: 8),
                        DistrictSchoolId = c.String(),
                    })
                .PrimaryKey(t => t.SchoolId);
            
            CreateTable(
                "dbo.Scores",
                c => new
                    {
                        ScoreId = c.Int(nullable: false, identity: true),
                        AssignmentId = c.Int(nullable: false),
                        DoubleValue = c.Double(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ScoreId);
            
            CreateTable(
                "dbo.AccessTokens",
                c => new
                    {
                        AccessTokenId = c.Int(nullable: false, identity: true),
                        LastUpdated = c.DateTime(nullable: false),
                        ProviderName = c.String(),
                        Token = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AccessTokenId);
            
            CreateTable(
                "dbo.Tenants",
                c => new
                    {
                        TenantId = c.Int(nullable: false, identity: true),
                        ClientId = c.String(),
                        LongLivedSessionToken = c.String(),
                        SharedSecret = c.String(),
                        inBloomTenantId = c.String(nullable: false, maxLength: 256),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TenantId);
            
            CreateTable(
                "dbo.CourseEnrolledUsers",
                c => new
                    {
                        CourseId = c.Int(nullable: false),
                        EnrolledUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.CourseId, t.EnrolledUserId })
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.EnrolledUserId, cascadeDelete: true)
                .Index(t => t.CourseId)
                .Index(t => t.EnrolledUserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.CourseEnrolledUsers", new[] { "EnrolledUserId" });
            DropIndex("dbo.CourseEnrolledUsers", new[] { "CourseId" });
            DropIndex("dbo.Courses", new[] { "School_SchoolId" });
            DropIndex("dbo.Courses", new[] { "District_DistrictId" });
            DropIndex("dbo.Courses", new[] { "State_StateId" });
            DropIndex("dbo.Courses", new[] { "Instructor_UserId" });
            DropIndex("dbo.Assignments", new[] { "Course_CourseId" });
            DropForeignKey("dbo.CourseEnrolledUsers", "EnrolledUserId", "dbo.Users");
            DropForeignKey("dbo.CourseEnrolledUsers", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Courses", "School_SchoolId", "dbo.Schools");
            DropForeignKey("dbo.Courses", "District_DistrictId", "dbo.Districts");
            DropForeignKey("dbo.Courses", "State_StateId", "dbo.States");
            DropForeignKey("dbo.Courses", "Instructor_UserId", "dbo.Users");
            DropForeignKey("dbo.Assignments", "Course_CourseId", "dbo.Courses");
            DropTable("dbo.CourseEnrolledUsers");
            DropTable("dbo.Tenants");
            DropTable("dbo.AccessTokens");
            DropTable("dbo.Scores");
            DropTable("dbo.Schools");
            DropTable("dbo.Districts");
            DropTable("dbo.States");
            DropTable("dbo.Users");
            DropTable("dbo.Courses");
            DropTable("dbo.Assignments");
        }
    }
}
