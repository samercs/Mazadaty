namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MessagesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Body = c.String(nullable: false),
                        ReceiverId = c.String(nullable: false, maxLength: 128),
                        IsNew = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(),
                        DeletedUtc = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
           
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Messages", new[] { "UserId" });
            DropTable("dbo.Messages");          
        }
    }
}
