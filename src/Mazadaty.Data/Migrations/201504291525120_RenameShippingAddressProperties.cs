namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameShippingAddressProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShipmentAddress", "Name", c => c.String(nullable: false));
            AddColumn("dbo.ShipmentAddress", "PhoneCountryCode", c => c.String(nullable: false));
            AddColumn("dbo.ShipmentAddress", "PhoneLocalNumber", c => c.String(nullable: false));
            DropColumn("dbo.ShipmentAddress", "FirstName");
            DropColumn("dbo.ShipmentAddress", "LastName");
            DropColumn("dbo.ShipmentAddress", "Email");
            DropColumn("dbo.ShipmentAddress", "PhoneNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShipmentAddress", "PhoneNumber", c => c.String(nullable: false));
            AddColumn("dbo.ShipmentAddress", "Email", c => c.String(nullable: false));
            AddColumn("dbo.ShipmentAddress", "LastName", c => c.String(nullable: false));
            AddColumn("dbo.ShipmentAddress", "FirstName", c => c.String(nullable: false));
            DropColumn("dbo.ShipmentAddress", "PhoneLocalNumber");
            DropColumn("dbo.ShipmentAddress", "PhoneCountryCode");
            DropColumn("dbo.ShipmentAddress", "Name");
        }
    }
}
