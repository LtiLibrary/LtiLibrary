namespace Provider.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Schema : DbMigration
    {
        public override void Up()
        {
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
                "dbo.PairedUsers",
                c => new
                    {
                        PairedUserId = c.Int(nullable: false, identity: true),
                        ConsumerId = c.Int(nullable: false),
                        ConsumerUserId = c.String(),
                        User_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.PairedUserId)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Email = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
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
                "dbo.LtiInboundRequests",
                c => new
                    {
                        LtiInboundRequestId = c.Int(nullable: false, identity: true),
                        ConsumerId = c.Int(nullable: false),
                        Nonce = c.String(),
                        OutcomeId = c.Int(nullable: false),
                        Timestamp = c.Long(nullable: false),
                        ContextId = c.String(),
                        ContextLabel = c.String(),
                        ContextTitle = c.String(),
                        ContextType = c.Int(),
                        CustomParametersAsQuerystring = c.String(),
                        LaunchPresentationLocale = c.String(),
                        LaunchPresentationCssUrl = c.String(),
                        LaunchPresentationDocumentTarget = c.Int(),
                        LaunchPresentationHeight = c.Int(),
                        LaunchPresentationReturnUrl = c.String(),
                        LaunchPresentationWidth = c.Int(),
                        LisOutcomeServiceUrl = c.String(),
                        LisPersonNameGiven = c.String(),
                        LisPersonNameFamily = c.String(),
                        LisPersonNameFull = c.String(),
                        LisPersonContactEmailPrimary = c.String(),
                        LisResultSourcedId = c.String(),
                        LtiMessageType = c.String(),
                        LtiVersion = c.String(),
                        ResourceLinkDescription = c.String(),
                        ResourceLinkId = c.String(),
                        RolesAsString = c.String(),
                        RoleScopeMentor = c.String(),
                        ResourceLinkTitle = c.String(),
                        ToolConsumerInfoProductFamilyCode = c.String(),
                        ToolConsumerInfoVersion = c.String(),
                        ToolConsumerInstanceContactEmail = c.String(),
                        ToolConsumerInstanceDescription = c.String(),
                        ToolConsumerInstanceGuid = c.String(),
                        ToolConsumerInstanceName = c.String(),
                        ToolConsumerInstanceUrl = c.String(),
                        Url = c.String(),
                        UserId = c.String(),
                        UserImage = c.String(),
                    })
                .PrimaryKey(t => t.LtiInboundRequestId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Outcomes", new[] { "Tool_ToolId" });
            DropIndex("dbo.PairedUsers", new[] { "User_UserId" });
            DropForeignKey("dbo.Outcomes", "Tool_ToolId", "dbo.Tools");
            DropForeignKey("dbo.PairedUsers", "User_UserId", "dbo.Users");
            DropTable("dbo.LtiInboundRequests");
            DropTable("dbo.Consumers");
            DropTable("dbo.Outcomes");
            DropTable("dbo.Tools");
            DropTable("dbo.States");
            DropTable("dbo.Schools");
            DropTable("dbo.Users");
            DropTable("dbo.PairedUsers");
            DropTable("dbo.Districts");
        }
    }
}
