using OrangeJetpack.Base.Core.Formatting;

namespace Mzayad.Data.Migrations
{
    using Models;
    using Models.Enum;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;

    public partial class InsertNewTrophies : DbMigration
    {
        public override void Up()
        {
            var insertTrophy = "insert into Trophies(TrophyId,[Key],Name,Description,IconUrl,XpAward,CreatedUtc)"
                             + "values ({TrophyId},{TrophyId},'{Name}','{Description}','{IconUrl}',{XpAward},'{CreatedUtc}')";
            foreach (var trophy in GetTrophiesList())
            {
                Sql(StringFormatter.ObjectFormat(insertTrophy, trophy));
            }
        }

        public override void Down()
        {
            var deleteTrophy = "delete from Trophies Where TrophyId = {TrophyId}";
            foreach (var trophy in GetTrophiesList())
            {
                Sql(StringFormatter.ObjectFormat(deleteTrophy, trophy));
            }
        }
        private List<Trophy> GetTrophiesList()
        {
            var trophoies = new List<Trophy>
            {
                new Trophy(){TrophyId = (int)TrophyKey.Bid25, Key=TrophyKey.Bid25, Name = "[{''k'':''en'',''v'':''Bid 25''}]",Description = "[{''k'':''en'',''v'':''Bid 25''}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.Bid5000, Key=TrophyKey.Bid5000, Name = "[{''k'':''en'',''v'':''Bid 5000''}]",Description = "[{''k'':''en'',''v'':''Bid 5000''}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.AutoBid25, Key=TrophyKey.AutoBid25, Name = "[{''k'':''en'',''v'':''Auto Bid 25''}]",Description = "[{''k'':''en'',''v'':''Auto Bid 25''}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.AutoBid5000, Key=TrophyKey.AutoBid5000, Name = "[{''k'':''en'',''v'':''Auto Bid 5000''}]",Description = "[{''k'':''en'',''v'':''Auto Bid 5000''}]",IconUrl = "",XpAward = 0},

            };

            trophoies.ForEach(i => i.IconUrl = "http://fontawesome.io/icon/trophy/");
            trophoies.ForEach(i => i.CreatedUtc = DateTime.UtcNow);

            return trophoies;
        }
    }
}
