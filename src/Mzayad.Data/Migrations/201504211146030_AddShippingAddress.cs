namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShippingAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "FirstName", c => c.String());
            AddColumn("dbo.Addresses", "LastName", c => c.String());
            AddColumn("dbo.Addresses", "Email", c => c.String());
            AddColumn("dbo.Addresses", "PhoneNumber", c => c.String());
            AddColumn("dbo.Addresses", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Addresses", "Discriminator");
            DropColumn("dbo.Addresses", "PhoneNumber");
            DropColumn("dbo.Addresses", "Email");
            DropColumn("dbo.Addresses", "LastName");
            DropColumn("dbo.Addresses", "FirstName");
        }
    }
}
