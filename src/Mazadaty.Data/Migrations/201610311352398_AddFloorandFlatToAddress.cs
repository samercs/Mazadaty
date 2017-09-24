namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFloorandFlatToAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "Floor", c => c.String(maxLength: 50));
            AddColumn("dbo.Addresses", "FlatNumber", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Addresses", "FlatNumber");
            DropColumn("dbo.Addresses", "Floor");
        }
    }
}
