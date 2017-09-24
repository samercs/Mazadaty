namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAllowPhoneSmsToOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "AllowPhoneSms", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "AllowPhoneSms");
        }
    }
}
