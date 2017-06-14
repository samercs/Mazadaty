namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTokenLogUsage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TokenLogs", "Usage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TokenLogs", "Usage");
        }
    }
}
