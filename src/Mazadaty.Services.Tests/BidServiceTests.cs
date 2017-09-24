using Mazadaty.Models;
using Mazadaty.Services.Tests.Fakes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mazadaty.Services.Tests
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable PossibleNullReferenceException

    [TestFixture]
    public class BidServiceTests
    {
        private static BidService GetBidService(IEnumerable<Bid> bids)
        {
            var dc = new InMemoryDataContext();
            foreach (var bid in bids)
            {
                dc.Bids.Add(bid);
            }

            return new BidService(new InMemoryDataContextFactory(dc), null);
        }

        [Test]
        public async Task GetConsecutiveBidDays_3ConsecutiveDays_Returns3()
        {
            var bidService = GetBidService(new[]
            {
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-1)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-2)}
            });
            var result = await bidService.GetConsecutiveBidDays(Constants.AnyUserId);

            Assert.AreEqual(3, result);
        }

        [Test]
        public async Task GetConsecutiveBidDays_3DaysWithDuplicateDays_Returns3()
        {
            var bidService = GetBidService(new[]
            {
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-1)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-2)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-2)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-2)}
            });
            var result = await bidService.GetConsecutiveBidDays(Constants.AnyUserId);

            Assert.AreEqual(3, result);
        }

        [Test]
        public async Task GetConsecutiveBidDays_3DaysWithGap_Returns2()
        {
            var bidService = GetBidService(new[]
            {
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-1)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-3)}
            });
            var result = await bidService.GetConsecutiveBidDays(Constants.AnyUserId);

            Assert.AreEqual(2, result);
        }

        [Test]
        public async Task GetConsecutiveBidDays_NoBids_Returns0()
        {
            var bidService = GetBidService(new Bid[0]);
            var result = await bidService.GetConsecutiveBidDays(Constants.AnyUserId);

            Assert.AreEqual(0, result);
        }

        [Test]
        public async Task CountUserBids_FromDate_Returns2()
        {
            var bidService = GetBidService(new[]
            {
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-1)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-3)}
            });
            var result = await bidService.CountUserBids(Constants.AnyUserId);

            Assert.AreEqual(3, result);
        }


        [Test]
        public async Task CountUserBids_NoFromDate_Returns3()
        {
            var bidService = GetBidService(new[]
            {
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-1)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = DateTime.UtcNow.AddDays(-3)}
            });
            var result = await bidService.CountUserBids(Constants.AnyUserId);

            Assert.AreEqual(3, result);
        }
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore PossibleNullReferenceException
}
