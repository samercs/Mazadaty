namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddKnetTransaction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KnetTransactions",
                c => new
                    {
                        PaymentTransactionId = c.Int(nullable: false, identity: true),
                        PaymentId = c.String(nullable: false, maxLength: 255),
                        Result = c.String(maxLength: 255),
                        AuthorizationNumber = c.String(maxLength: 255),
                        ReferenceNumber = c.String(maxLength: 255),
                        PostDate = c.String(maxLength: 255),
                        TransactionId = c.String(maxLength: 255),
                        TrackId = c.String(maxLength: 255),
                        OrderId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        RequestParams = c.String(),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentTransactionId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.PaymentId, unique: true)
                .Index(t => t.OrderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KnetTransactions", "OrderId", "dbo.Orders");
            DropIndex("dbo.KnetTransactions", new[] { "OrderId" });
            DropIndex("dbo.KnetTransactions", new[] { "PaymentId" });
            DropTable("dbo.KnetTransactions");
        }
    }
}
