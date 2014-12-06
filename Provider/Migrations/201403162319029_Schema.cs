namespace Provider.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Schema : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Consumers",
                c => new
                    {
                        ConsumerId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Key = c.String(nullable: false),
                        Secret = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ConsumerId);
            
            CreateTable(
                "dbo.Outcomes",
                c => new
                    {
                        OutcomeId = c.Int(nullable: false, identity: true),
                        ConsumerId = c.Int(nullable: false),
                        ContextTitle = c.String(),
                        LisResultSourcedId = c.String(),
                        ServiceUrl = c.String(),
                        Tool_ToolId = c.Int(),
                    })
                .PrimaryKey(t => t.OutcomeId)
                .ForeignKey("dbo.Tools", t => t.Tool_ToolId)
                .Index(t => t.Tool_ToolId);
            
            CreateTable(
                "dbo.PairedUsers",
                c => new
                    {
                        PairedUserId = c.Int(nullable: false, identity: true),
                        ConsumerId = c.Int(nullable: false),
                        ConsumerUserId = c.String(),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.PairedUserId);
            
            CreateTable(
                "dbo.ProviderRequests",
                c => new
                    {
                        ProviderRequestId = c.Int(nullable: false, identity: true),
                        LtiRequest = c.String(),
                        Received = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ProviderRequestId);
            
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
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Tools",
                c => new
                    {
                        ToolId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.ToolId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
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
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Outcomes", "Tool_ToolId", "dbo.Tools");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Outcomes", new[] { "Tool_ToolId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Tools");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ProviderRequests");
            DropTable("dbo.PairedUsers");
            DropTable("dbo.Outcomes");
            DropTable("dbo.Consumers");
        }
    }
}
