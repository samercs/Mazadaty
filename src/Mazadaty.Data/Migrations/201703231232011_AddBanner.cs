namespace Mazadaty.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBanner : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Banners",
                c => new
                    {
                        BannerId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        SecondaryTitle = c.String(nullable: false),
                        OrginalImgUrl = c.String(),
                        ImgSmUrl = c.String(),
                        ImgMdUrl = c.String(),
                        ImgLgUrl = c.String(),
                        Url = c.String(),
                        Status = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(),
                        DeletedUtc = c.DateTime(),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.BannerId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Banners");
        }
    }
}
