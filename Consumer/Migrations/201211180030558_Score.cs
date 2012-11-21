namespace Consumer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Score : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Scores",
                c => new
                    {
                        ScoreId = c.Int(nullable: false, identity: true),
                        AssignmentId = c.Int(nullable: false),
                        DecimalValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ScoreId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Scores");
        }
    }
}
