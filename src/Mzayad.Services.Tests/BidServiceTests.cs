using Mzayad.Models;
using Mzayad.Services.Tests.Fakes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mzayad.Services.Tests
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

            var bidService = new BidService(new InMemoryDataContextFactory(dc));
            return bidService;
        }

        [Test]
        public async Task GetConsecutiveBidDays_3ConsecutiveDays_Returns3()
        {
            var bidService = GetBidService(new[]
            {
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = new DateTime(2000, 1, 1)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = new DateTime(2000, 1, 2)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = new DateTime(2000, 1, 3)}
            });
            var result = await bidService.GetConsecutiveBidDays(Constants.AnyUserId);

            Assert.AreEqual(3, result);
        }

        [Test]
        public async Task GetConsecutiveBidDays_3DaysWithDuplicateDays_Returns3()
        {
            var bidService = GetBidService(new[]
            {
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = new DateTime(2000, 1, 1)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = new DateTime(2000, 1, 1)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = new DateTime(2000, 1, 1)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = new DateTime(2000, 1, 2)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = new DateTime(2000, 1, 3)}
            });
            var result = await bidService.GetConsecutiveBidDays(Constants.AnyUserId);

            Assert.AreEqual(3, result);
        }

        [Test]
        public async Task GetConsecutiveBidDays_3DaysWithGap_Returns2()
        {
            var bidService = GetBidService(new[]
            {
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = new DateTime(2000, 1, 1)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = new DateTime(2000, 1, 3)},
                new Bid {UserId = Constants.AnyUserId, CreatedUtc = new DateTime(2000, 1, 4)}
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
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore PossibleNullReferenceException
}