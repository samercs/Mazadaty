namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserPhoneAddress : DbMigration
    {
        public override void Up()
        {
            Sql("update dbo.AspNetUsers set PhoneNumber = '123456789' where PhoneNumber is null;");
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        AddressLine1 = c.String(nullable: false),
                        AddressLine2 = c.String(),
                        AddressLine3 = c.String(),
                        AddressLine4 = c.String(),
                        CityArea = c.String(nullable: false),
                        StateProvince = c.String(),
                        PostalCode = c.String(),
                        CountryCode = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AddressId);
            
            AddColumn("dbo.AspNetUsers", "PhoneCountryCode", c => c.String(nullable: false, maxLength: 3));
            AddColumn("dbo.AspNetUsers", "AddressId", c => c.Int());
            AlterColumn("dbo.AspNetUsers", "PhoneNumber", c => c.String(nullable: false, maxLength: 12));
            CreateIndex("dbo.AspNetUsers", "AddressId");
            AddForeignKey("dbo.AspNetUsers", "AddressId", "dbo.Addresses", "AddressId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "AddressId", "dbo.Addresses");
            DropIndex("dbo.AspNetUsers", new[] { "AddressId" });
            AlterColumn("dbo.AspNetUsers", "PhoneNumber", c => c.String());
            DropColumn("dbo.AspNetUsers", "AddressId");
            DropColumn("dbo.AspNetUsers", "PhoneCountryCode");
            DropTable("dbo.Addresses");
        }
    }
}
