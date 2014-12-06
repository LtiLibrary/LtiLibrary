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
                        Name = c.String(nullable: false),
                        Instructor_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CourseId)
                .ForeignKey("dbo.AspNetUsers", t => t.Instructor_Id)
                .Index(t => t.Instructor_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        SendEmail = c.Boolean(),
                        SendName = c.Boolean(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Course_CourseId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.Course_CourseId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.Course_CourseId);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ContentItemTools",
                c => new
                    {
                        ContentItemToolId = c.Int(nullable: false, identity: true),
                        ConsumerKey = c.String(nullable: false),
                        ConsumerSecret = c.String(nullable: false),
                        CustomParameters = c.String(),
                        Description = c.String(),
                        Name = c.String(nullable: false),
                        Url = c.String(nullable: false),
                        Owner_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ContentItemToolId)
                .ForeignKey("dbo.AspNetUsers", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Scores",
                c => new
                    {
                        ScoreId = c.Int(nullable: false, identity: true),
                        AssignmentId = c.Int(nullable: false),
                        DoubleValue = c.Double(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.ScoreId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ContentItemTools", "Owner_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Courses", "Instructor_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "Course_CourseId", "dbo.Courses");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Assignments", "Course_CourseId", "dbo.Courses");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ContentItemTools", new[] { "Owner_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Course_CourseId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Courses", new[] { "Instructor_Id" });
            DropIndex("dbo.Assignments", new[] { "Course_CourseId" });
            DropTable("dbo.Scores");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ContentItemTools");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Courses");
            DropTable("dbo.Assignments");
        }
    }
}
