using Mzayad.Models;
using Mzayad.Services.Tests.Fakes;
using NUnit.Framework;
using System;

namespace Mzayad.Services.Tests
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable PossibleNullReferenceException

    [TestFixture]
    public class AutoBidServiceTests
    {
        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, true)]
        [TestCase(4, true)]
        [TestCase(5, true)]
        [TestCase(6, true)]
        [TestCase(7, true)]
        [TestCase(8, true)]
        [TestCase(9, true)]
        [TestCase(10, true)]
        [TestCase(11, true)]
        [TestCase(12, true)]
        [TestCase(13, false)]
        [TestCase(14, true)]
        [TestCase(15, false)]
        [TestCase(16, true)]
        [TestCase(48, true)]
        [TestCase(49, false)]
        [TestCase(50, false)]
        [TestCase(51, false)]
        [TestCase(56, true)]
        [TestCase(96, true)]
        [TestCase(192, true)]
        [TestCase(208, false)]
        [TestCase(224, true)]
        [TestCase(1280, true)]
        public void TryGetAutoBid_UserWithMaxBid_ReturnsExpected(int secondsLeft, bool expectedNotNull)
        {
            var dc = new InMemoryDataContext();
            dc.AutoBids.Add(new AutoBid
            {
                AuctionId = Constants.AnyInt,
                UserId = Constants.AnyUserId,
                User = new ApplicationUser
                {
                    Id = Constants.AnyUserId
                },
                MaxBid = decimal.MaxValue
            });

            var service = new AutoBidService(new InMemoryDataContextFactory(dc));
            var user = service.TryGetAutoBid(Constants.AnyInt, secondsLeft, 0);

            Assert.AreEqual(user != null, expectedNotNull);
        }

        [Test]
        public void TryGetAutoBid_OneAutoBidWithMaxLessThanLastBidAmount_ReturnsNull()
        {
            const int lastBidAmount = 10;

            var dc = new InMemoryDataContext();
            dc.AutoBids.Add(new AutoBid
            {
                AuctionId = Constants.AnyInt,
                UserId = Constants.AnyUserId,
                User = new ApplicationUser
                {
                    Id = Constants.AnyUserId
                },
                MaxBid = lastBidAmount
            });

            var service = new AutoBidService(new InMemoryDataContextFactory(dc));
            var user = service.TryGetAutoBid(Constants.AnyInt, 1, lastBidAmount);

            Assert.IsNull(user);
        }
        
        [Test]
        public void CountUserAutoBids_FromDate_Returns2()
        {
            var dc = new InMemoryDataContext();
            var auctionId1 = Constants.AnyInt;
            var auctionId2 = Constants.AnyInt  + 1;
            dc.AutoBids.Add(new AutoBid
            {
                AuctionId = auctionId1,
                UserId = Constants.AnyUserId,
                CreatedUtc = DateTime.UtcNow.AddMinutes(-30)
            });
            dc.AutoBids.Add(new AutoBid
            {
                AuctionId = auctionId1,
                UserId = Constants.AnyUserId,
                CreatedUtc = DateTime.UtcNow.AddMinutes(-2)
            });
            dc.AutoBids.Add(new AutoBid
            {
                AuctionId = auctionId2,
                UserId = Constants.AnyUserId,
                CreatedUtc = DateTime.UtcNow.AddHours(-2)
            });

            var service = new AutoBidService(new InMemoryDataContextFactory(dc));
            var result = service.CountUserAutoBids(Constants.AnyUserId, DateTime.UtcNow.AddDays(-1)).Result;

            Assert.AreEqual(2, result);
        }
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore PossibleNullReferenceException
}