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
        [Test]
        public void TryGetAutoBid_UserWithMaxBid_ReturnsExpected()
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
            var userId = service.TryGetAutoBid(Constants.AnyInt, "", 0);

            Assert.AreEqual(userId, Constants.AnyUserId);
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
            var user = service.TryGetAutoBid(Constants.AnyInt, "", lastBidAmount);

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