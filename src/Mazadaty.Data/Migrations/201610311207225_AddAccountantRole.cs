namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAccountantRole : DbMigration
    {
        public override void Up()
        {
            Sql("insert into dbo.AspNetRoles (Id, Name) values ('" + Guid.NewGuid().ToString().ToLower() + "', 'Accountant');");
        }
        
        public override void Down()
        {
            Sql("delete from dbo.AspNetUserRoles where RoleId = (select id from AspNetRoles where Name='Accountant')");
            Sql("delete from dbo.AspNetRoles where Name='Accountant'");
        }
    }
}
