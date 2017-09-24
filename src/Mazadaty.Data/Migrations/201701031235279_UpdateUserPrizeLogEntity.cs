namespace Mazadaty.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UpdateUserPrizeLogEntity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserPrizeLogs", "PrizeId", "dbo.Prizes");
            DropIndex("dbo.UserPrizeLogs", new[] { "PrizeId" });
            AlterColumn("dbo.UserPrizeLogs", "PrizeId", c => c.Int());
            CreateIndex("dbo.UserPrizeLogs", "PrizeId");
            AddForeignKey("dbo.UserPrizeLogs", "PrizeId", "dbo.Prizes", "PrizeId");
        }

        public override void Down()
        {
            Sql("delete from dbo.UserPrizeLogs");
            DropForeignKey("dbo.UserPrizeLogs", "PrizeId", "dbo.Prizes");
            DropIndex("dbo.UserPrizeLogs", new[] { "PrizeId" });
            AlterColumn("dbo.UserPrizeLogs", "PrizeId", c => c.Int(nullable: false));
            CreateIndex("dbo.UserPrizeLogs", "PrizeId");
            AddForeignKey("dbo.UserPrizeLogs", "PrizeId", "dbo.Prizes", "PrizeId", cascadeDelete: true);
        }
    }
}
