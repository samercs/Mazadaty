using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services.Tests.Fakes;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mzayad.Models.Enums;

namespace Mzayad.Services.Tests
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable PossibleNullReferenceException

    [TestFixture]
    public class AuctionServiceTests
    {
   
        

        [Test]
        public void Method_Scenario_Expected()
        {
            var url = "http://localhost:15189/ar/user/dashboard";
            var lang = "en/";

            var re = new Regex(@"/en|ar/");

            var actual = re.Replace(url, lang);

            Assert.AreEqual("http://localhost:15189/en/user/dashboard", actual);
        }


        [Test]
        public async Task GetCurrentAuctions_NoAuctions_ReturnsEmptyList()
        {
            var auctionService = new AuctionService(new InMemoryDataContextFactory());

            var results = await auctionService.GetCurrentAuctions();

            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public async Task GetCurrentAuctions_OnlyHiddenAuctions_ReturnsEmptyList()
        {
            var dc = new InMemoryDataContext();
            dc.Auctions.Add(new Auction {Status = AuctionStatus.Hidden});
            dc.Auctions.Add(new Auction {Status = AuctionStatus.Hidden});
            dc.Auctions.Add(new Auction {Status = AuctionStatus.Hidden});
            
            var auctionService = new AuctionService(new InMemoryDataContextFactory(dc));

            var results = await auctionService.GetCurrentAuctions();

            Assert.AreEqual(0, results.Count());
        }

        [Test]
        public async Task GetCurrentAuctions_MixOfPublicAndHiddenAuctions_ReturnsOnlyPublicAuctionc()
        {
            var dc = new InMemoryDataContext();
            dc.Auctions.Add(new Auction { Status = AuctionStatus.Public });
            dc.Auctions.Add(new Auction { Status = AuctionStatus.Public });
            dc.Auctions.Add(new Auction { Status = AuctionStatus.Hidden });
            dc.Auctions.Add(new Auction { Status = AuctionStatus.Hidden });

            var auctionService = new AuctionService(new InMemoryDataContextFactory(dc));

            var results = await auctionService.GetCurrentAuctions();

            Assert.AreEqual(2, results.Count());
        }

        [Test]
        public void CountAuctionsWon_FromDate_Returns2()
        {
            var dc = new InMemoryDataContext();
            var auctionId1 = Constants.AnyInt;
            var auctionId2 = Constants.AnyInt + 1;
            dc.Auctions.Add(new Auction
            {
                AuctionId = auctionId1,
                WonByUserId = Constants.AnyUserId,
                CreatedUtc = DateTime.UtcNow.AddDays(-1)
            });
            dc.Auctions.Add(new Auction
            {
                AuctionId = auctionId2,
                WonByUserId = Constants.AnyUserId,
                CreatedUtc = DateTime.UtcNow.AddDays(-4)
            });

            var service = new AuctionService(new InMemoryDataContextFactory(dc));
            var result = service.CountAuctionsWon(Constants.AnyUserId, DateTime.UtcNow.AddDays(-7)).Result;

            Assert.AreEqual(2, result);
        }
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore PossibleNullReferenceException
}
