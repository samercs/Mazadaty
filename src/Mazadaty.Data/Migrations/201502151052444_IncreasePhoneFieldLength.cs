namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncreasePhoneFieldLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "PhoneCountryCode", c => c.String(nullable: false, maxLength: 4));
            AlterColumn("dbo.AspNetUsers", "PhoneNumber", c => c.String(nullable: false, maxLength: 15));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "PhoneNumber", c => c.String(nullable: false, maxLength: 12));
            AlterColumn("dbo.AspNetUsers", "PhoneCountryCode", c => c.String(nullable: false, maxLength: 3));
        }
    }
}
