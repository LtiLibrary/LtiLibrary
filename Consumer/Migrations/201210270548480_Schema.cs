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
                        CustomParameters = c.String(),
                        Description = c.String(),
                        LtiVersionId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Secret = c.String(),
                        SharingScopeId = c.Int(nullable: false),
                        Url = c.String(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AssignmentId)
                .ForeignKey("dbo.LtiVersions", t => t.LtiVersionId, cascadeDelete: true)
                .ForeignKey("dbo.SharingScopes", t => t.SharingScopeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.LtiVersionId)
                .Index(t => t.SharingScopeId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.LtiVersions",
                c => new
                    {
                        LtiVersionId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.LtiVersionId);
            
            CreateTable(
                "dbo.SharingScopes",
                c => new
                    {
                        SharingScopeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.SharingScopeId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        DistrictId = c.String(maxLength: 8),
                        Email = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        SchoolId = c.String(maxLength: 12),
                        SendEmail = c.Boolean(),
                        SendName = c.Boolean(),
                        StateId = c.String(maxLength: 2),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Districts", t => t.DistrictId)
                .ForeignKey("dbo.Schools", t => t.SchoolId)
                .ForeignKey("dbo.States", t => t.StateId)
                .Index(t => t.DistrictId)
                .Index(t => t.SchoolId)
                .Index(t => t.StateId);
            
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
                "dbo.States",
                c => new
                    {
                        StateId = c.String(nullable: false, maxLength: 2),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.StateId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", new[] { "StateId" });
            DropIndex("dbo.Users", new[] { "SchoolId" });
            DropIndex("dbo.Users", new[] { "DistrictId" });
            DropIndex("dbo.Assignments", new[] { "UserId" });
            DropIndex("dbo.Assignments", new[] { "SharingScopeId" });
            DropIndex("dbo.Assignments", new[] { "LtiVersionId" });
            DropForeignKey("dbo.Users", "StateId", "dbo.States");
            DropForeignKey("dbo.Users", "SchoolId", "dbo.Schools");
            DropForeignKey("dbo.Users", "DistrictId", "dbo.Districts");
            DropForeignKey("dbo.Assignments", "UserId", "dbo.Users");
            DropForeignKey("dbo.Assignments", "SharingScopeId", "dbo.SharingScopes");
            DropForeignKey("dbo.Assignments", "LtiVersionId", "dbo.LtiVersions");
            DropTable("dbo.States");
            DropTable("dbo.Schools");
            DropTable("dbo.Districts");
            DropTable("dbo.Users");
            DropTable("dbo.SharingScopes");
            DropTable("dbo.LtiVersions");
            DropTable("dbo.Assignments");
        }
    }
}
