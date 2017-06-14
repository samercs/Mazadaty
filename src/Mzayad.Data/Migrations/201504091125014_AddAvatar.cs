namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAvatar : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Avatars",
                c => new
                    {
                        AvatarId = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        SortOrder = c.Double(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AvatarId);

            Sql("insert into Avatars (Url, SortOrder) values ('//az712326.vo.msecnd.net/assets/no-image-512x512-635627099896729695.png', 0)");
        }
        
        public override void Down()
        {
            DropTable("dbo.Avatars");
        }
    }
}
