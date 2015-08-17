using System;
using System.Collections.Generic;
using Mzayad.Models;
using OrangeJetpack.Base.Data;
using System.Data.Entity.Migrations;
using Mzayad.Models.Enum;

namespace Mzayad.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SqlClient", new MigrationSqlGenerator());
        }

        protected override void Seed(DataContext context)
        {
            var trophoies = new List<Trophy>
            {
                new Trophy(){TrophyId = (int)TrophyKey.BidOnNewYear, Key=TrophyKey.BidOnNewYear, Name = "[{'k':'en','v':'Bid on new year'}]",Description = "[{'k':'en','v':'Bid on new year'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BidOnIslamicNewYear, Key=TrophyKey.BidOnIslamicNewYear, Name = "[{'k':'en','v':'Bid on Islamic new year'}]",Description = "[{'k':'en','v':'Bid on Islamic new year'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BidOnEid, Key=TrophyKey.BidOnEid, Name = "[{'k':'en','v':'Bid on Eid'}]",Description = "[{'k':'en','v':'Bid on Eid'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BidOnAnniversary, Key=TrophyKey.BidOnAnniversary, Name = "[{'k':'en','v':'Bid on anniversary'}]",Description = "[{'k':'en','v':'Bid on anniversary'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.ReturnAfterInactivity, Key=TrophyKey.ReturnAfterInactivity, Name = "[{'k':'en','v':'Return after in-activity'}]",Description = "[{'k':'en','v':'Return after in-activity'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.BidDayStreak3, Key=TrophyKey.BidDayStreak3, Name = "[{'k':'en','v':'Bid day streak 3'}]",Description = "[{'k':'en','v':'Bid day streak 3'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BidDayStreak7, Key=TrophyKey.BidDayStreak7, Name = "[{'k':'en','v':'Bid day streak 7'}]",Description = "[{'k':'en','v':'Bid day streak 7'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BidDayStreak30, Key=TrophyKey.BidDayStreak30, Name = "[{'k':'en','v':'Bid day streak 30'}]",Description = "[{'k':'en','v':'Bid day streak 30'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BidDayStreak90, Key=TrophyKey.BidDayStreak90, Name = "[{'k':'en','v':'Bid day streak 90'}]",Description = "[{'k':'en','v':'Bid day streak 90'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BidDayStreak180, Key=TrophyKey.BidDayStreak180, Name = "[{'k':'en','v':'Bid day streak 180'}]",Description = "[{'k':'en','v':'Bid day streak 180'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BidDayStreak365, Key=TrophyKey.BidDayStreak365, Name = "[{'k':'en','v':'Bid day streak 365'}]",Description = "[{'k':'en','v':'Bid day streak 365'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.BidItemStreak3, Key=TrophyKey.BidItemStreak3, Name = "[{'k':'en','v':'Bid item streak 3'}]",Description = "[{'k':'en','v':'Bid item streak '}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BidItemStreak5, Key=TrophyKey.BidItemStreak5, Name = "[{'k':'en','v':'Bid item streak 5'}]",Description = "[{'k':'en','v':'Bid item streak 5'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BidItemStreak10, Key=TrophyKey.BidItemStreak10, Name = "[{'k':'en','v':'Bid item streak 10'}]",Description = "[{'k':'en','v':'Bid item streak 10'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BidItemStreak15, Key=TrophyKey.BidItemStreak15, Name = "[{'k':'en','v':'Bid item streak 15'}]",Description = "[{'k':'en','v':'Bid item streak 15'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.WinItemStreak2, Key=TrophyKey.WinItemStreak2, Name = "[{'k':'en','v':'Win item streak 2'}]",Description = "[{'k':'en','v':'Win item streak 2'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.WinItemStreak4, Key=TrophyKey.WinItemStreak4, Name = "[{'k':'en','v':'Win item streak 4'}]",Description = "[{'k':'en','v':'Win item streak 4'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.WinItemStreak8, Key=TrophyKey.WinItemStreak8, Name = "[{'k':'en','v':'Win item streak 8'}]",Description = "[{'k':'en','v':'Win item streak 8'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.WinItemStreak15, Key=TrophyKey.WinItemStreak15, Name = "[{'k':'en','v':'Win item streak 15'}]",Description = "[{'k':'en','v':'Win item streak '}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.WinDayStreak3, Key=TrophyKey.WinDayStreak3, Name = "[{'k':'en','v':'Win day streak 3'}]",Description = "[{'k':'en','v':'Win day streak 3'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.WinDayStreak7, Key=TrophyKey.WinDayStreak7, Name = "[{'k':'en','v':'Win day streak 7'}]",Description = "[{'k':'en','v':'Win day streak 7'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.WinDayStreak30, Key=TrophyKey.WinDayStreak30, Name = "[{'k':'en','v':'Win day streak 30'}]",Description = "[{'k':'en','v':'Win day streak 30'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.VisitDayStreak3, Key=TrophyKey.VisitDayStreak3, Name = "[{'k':'en','v':'Visit day streak 3'}]",Description = "[{'k':'en','v':'Visit day streak 3'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.VisitDayStreak7, Key=TrophyKey.VisitDayStreak7, Name = "[{'k':'en','v':'Visit day streak 7'}]",Description = "[{'k':'en','v':'Visit day streak 7'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.VisitDayStreak30, Key=TrophyKey.VisitDayStreak30, Name = "[{'k':'en','v':'Visit day streak 30'}]",Description = "[{'k':'en','v':'Visit day streak 30'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.VisitDayStreak90, Key=TrophyKey.VisitDayStreak90, Name = "[{'k':'en','v':'Visit day streak 90'}]",Description = "[{'k':'en','v':'Visit day streak 90'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.VisitDayStreak180, Key=TrophyKey.VisitDayStreak180, Name = "[{'k':'en','v':'Visit day streak 180'}]",Description = "[{'k':'en','v':'Visit day streak 180'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.VisitDayStreak365, Key=TrophyKey.VisitDayStreak365, Name = "[{'k':'en','v':'Visit day streak 365'}]",Description = "[{'k':'en','v':'Visit day streak 365'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.VerifyProfile, Key=TrophyKey.VerifyProfile, Name = "[{'k':'en','v':'Verify profile'}]",Description = "[{'k':'en','v':'Verify profile'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.PurchaseSubscription, Key=TrophyKey.PurchaseSubscription, Name = "[{'k':'en','v':'Purchase subscription'}]",Description = "[{'k':'en','v':'Purchase subscription'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.WinAuction1, Key=TrophyKey.WinAuction1, Name = "[{'k':'en','v':'Win auction 1'}]",Description = "[{'k':'en','v':'Win auction 1'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.WinAuction5, Key=TrophyKey.WinAuction5, Name = "[{'k':'en','v':'Win auction 5'}]",Description = "[{'k':'en','v':'Win auction 5'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.WinAuction10, Key=TrophyKey.WinAuction10, Name = "[{'k':'en','v':'Win auction 10'}]",Description = "[{'k':'en','v':'Win auction 10'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.WinAuction20, Key=TrophyKey.WinAuction20, Name = "[{'k':'en','v':'Win auction 20'}]",Description = "[{'k':'en','v':'Win auction 20'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.WinAuction50, Key=TrophyKey.WinAuction50, Name = "[{'k':'en','v':'Win auction 50'}]",Description = "[{'k':'en','v':'Win auction 50'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.WinAuction100, Key=TrophyKey.WinAuction100, Name = "[{'k':'en','v':'Win auction 100'}]",Description = "[{'k':'en','v':'Win auction 100'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.BuyNow1, Key=TrophyKey.BuyNow1, Name = "[{'k':'en','v':'Buy now 1'}]",Description = "[{'k':'en','v':'Buy now 1'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BuyNow5, Key=TrophyKey.BuyNow5, Name = "[{'k':'en','v':'Buy now 5'}]",Description = "[{'k':'en','v':'Buy now 5'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BuyNow10, Key=TrophyKey.BuyNow10, Name = "[{'k':'en','v':'Buy now 10'}]",Description = "[{'k':'en','v':'Buy now 10'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BuyNow20, Key=TrophyKey.BuyNow20, Name = "[{'k':'en','v':'Buy now 20'}]",Description = "[{'k':'en','v':'Buy now 20'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BuyNow50, Key=TrophyKey.BuyNow50, Name = "[{'k':'en','v':'Buy now 50'}]",Description = "[{'k':'en','v':'Buy now 50'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.BuyNow100, Key=TrophyKey.BuyNow100, Name = "[{'k':'en','v':'Buy now 100'}]",Description = "[{'k':'en','v':'Buy now 100'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.Bid10, Key=TrophyKey.Bid10, Name = "[{'k':'en','v':'Bid 10'}]",Description = "[{'k':'en','v':'Bid 10'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.Bid50, Key=TrophyKey.Bid50, Name = "[{'k':'en','v':'Bid 50'}]",Description = "[{'k':'en','v':'Bid 50'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.Bid100, Key=TrophyKey.Bid100, Name = "[{'k':'en','v':'Bid 100'}]",Description = "[{'k':'en','v':'Bid 100'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.Bid250, Key=TrophyKey.Bid250, Name = "[{'k':'en','v':250'Bid '}]",Description = "[{'k':'en','v':'Bid 250'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.Bid500, Key=TrophyKey.Bid500, Name = "[{'k':'en','v':'Bid 500'}]",Description = "[{'k':'en','v':'Bid 500'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.Bid1000, Key=TrophyKey.Bid1000, Name = "[{'k':'en','v':'Bid 1000'}]",Description = "[{'k':'en','v':'Bid 1000'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.Bid2000, Key=TrophyKey.Bid2000, Name = "[{'k':'en','v':'Bid 2000'}]",Description = "[{'k':'en','v':'Bid 2000'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.AutoBid10, Key=TrophyKey.AutoBid10, Name = "[{'k':'en','v':'Auto Bid 10'}]",Description = "[{'k':'en','v':'Auto Bid 10'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.AutoBid50, Key=TrophyKey.AutoBid50, Name = "[{'k':'en','v':'Auto Bid 50'}]",Description = "[{'k':'en','v':'Auto Bid 50'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.AutoBid100, Key=TrophyKey.AutoBid100, Name = "[{'k':'en','v':'Auto Bid 100'}]",Description = "[{'k':'en','v':'Auto Bid 100'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.AutoBid250, Key=TrophyKey.AutoBid250, Name = "[{'k':'en','v':'Auto Bid 250'}]",Description = "[{'k':'en','v':'Auto Bid 250'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.AutoBid500, Key=TrophyKey.AutoBid500, Name = "[{'k':'en','v':'Auto Bid 500'}]",Description = "[{'k':'en','v':'Auto Bid 500'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.AutoBid1000, Key=TrophyKey.AutoBid1000, Name = "[{'k':'en','v':'Auto Bid 1000'}]",Description = "[{'k':'en','v':'Auto Bid 1000'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.AutoBid2000, Key=TrophyKey.AutoBid2000, Name = "[{'k':'en','v':'Auto Bid 2000'}]",Description = "[{'k':'en','v':'Auto Bid 2000'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.SpendOnAnything100, Key=TrophyKey.SpendOnAnything100, Name = "[{'k':'en','v':'Spend on anything 100'}]",Description = "[{'k':'en','v':'Spend on anything 100'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.SpendOnAnything500, Key=TrophyKey.SpendOnAnything500, Name = "[{'k':'en','v':'Spend on anything 500'}]",Description = "[{'k':'en','v':'Spend on anything 500'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.SpendOnAnything1000, Key=TrophyKey.SpendOnAnything1000, Name = "[{'k':'en','v':'Spend on anything 1000'}]",Description = "[{'k':'en','v':'Spend on anything 1000'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.SpendOnAnything2000, Key=TrophyKey.SpendOnAnything2000, Name = "[{'k':'en','v':'Spend on anything 2000'}]",Description = "[{'k':'en','v':'Spend on anything 2000'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.SpendOnAuctions100, Key=TrophyKey.SpendOnAuctions100, Name = "[{'k':'en','v':'Spend on auctions 100'}]",Description = "[{'k':'en','v':'Spend on auctions 100'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.SpendOnAuctions500, Key=TrophyKey.SpendOnAuctions500, Name = "[{'k':'en','v':'Spend on auctions 500'}]",Description = "[{'k':'en','v':'Spend on auctions 500'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.SpendOnAuctions1000, Key=TrophyKey.SpendOnAuctions1000, Name = "[{'k':'en','v':'Spend on auctions 1000'}]",Description = "[{'k':'en','v':'Spend on auctions 1000'}]",IconUrl = "",XpAward = 0},
                new Trophy(){TrophyId = (int)TrophyKey.SpendOnAuctions2000, Key=TrophyKey.SpendOnAuctions2000, Name = "[{'k':'en','v':'Spend on auctions 2000'}]",Description = "[{'k':'en','v':'Spend on auctions 2000'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.ReferFriends3 , Key=TrophyKey.ReferFriends3, Name = "[{'k':'en','v':'Refer friends 3'}]",Description = "[{'k':'en','v':'Refer friends 3'}]",IconUrl = "",XpAward = 0},

                new Trophy(){TrophyId = (int)TrophyKey.BigWinOnOneBid, Key=TrophyKey.BigWinOnOneBid, Name = "[{'k':'en','v':'Big win in one bid'}]",Description = "[{'k':'en','v':'Big win in one bid'}]",IconUrl = "",XpAward = 0},

            };
            trophoies.ForEach(i => i.IconUrl = "http://fontawesome.io/icon/trophy/");
            trophoies.ForEach(i => i.CreatedUtc = DateTime.UtcNow);
            trophoies.ForEach(i => context.Trophies.AddOrUpdate(i));
            context.SaveChanges();
        }
    }
}
