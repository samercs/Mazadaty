using Mzayad.Models;
using Mzayad.Models.Enum;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Localization;

namespace Mzayad.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompleteProfileTrophy : DbMigration
    {
        public override void Up()
        {
            var trophy = new Trophy
            {
                TrophyId = (int)TrophyKey.CompleteProfile,
                Key = TrophyKey.CompleteProfile,
                Name = (new[]
                {
                    new LocalizedContent("en",@"Complete Profile"),
                    new LocalizedContent("ar","@")
                }).Serialize(),
                Description = (new[]
                {
                    new LocalizedContent("en",@"Complete Profile"),
                    new LocalizedContent("ar","@")
                }).Serialize(),
                IconUrl = "https://az723232.vo.msecnd.net/assets/trophy-635755073480687086.png",
                XpAward = 0,
                CreatedUtc = DateTime.UtcNow
            };

            const string insertTrophy = "insert into Trophies(TrophyId,[Key],Name,Description,IconUrl,XpAward,CreatedUtc)"
                                        + "values ({TrophyId},{TrophyId},'{Name}','{Description}','{IconUrl}',{XpAward},'{CreatedUtc}')";
            Sql(StringFormatter.ObjectFormat(insertTrophy, trophy));
        }
        
        public override void Down()
        {
            Sql($"DELETE FROM dbo.Trophies WHERE TrophyId={(int)TrophyKey.CompleteProfile}");
        }
    }
}
