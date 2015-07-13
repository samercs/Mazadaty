using System;
using System.Threading.Tasks;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services.Tests.Fakes;
using NUnit.Framework;

namespace Mzayad.Services.Tests
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable PossibleNullReferenceException

    [TestFixture]
    public class SubscriptionServiceTests
    {
        [Test]
        public void ValidateSubscription_SubscriptionIsNull_ReturnsCorrectResult()
        {
            var result = SubscriptionService.ValidateSubscription(null);

            Assert.AreEqual(false, result.IsValid);
            Assert.AreEqual(SubscriptionValidationResult.ReasonType.Null, result.Reason);
        }

        [Test]
        public void ValidateSubscription_SubscriptionIsNotActive_ReturnsCorrectResult()
        {
            var subscription = new Subscription
            {
                Status = SubscriptionStatus.Disabled
            };

            var result = SubscriptionService.ValidateSubscription(subscription);

            Assert.AreEqual(false, result.IsValid);
            Assert.AreEqual(SubscriptionValidationResult.ReasonType.Disabled, result.Reason);
        }

        [Test]
        public void ValidateSubscription_SubscriptionIsExpired_ReturnsCorrectResult()
        {
            var subscription = new Subscription
            {
                Status = SubscriptionStatus.Active,
                ExpirationUtc = DateTime.UtcNow.AddDays(-1)
            };

            var result = SubscriptionService.ValidateSubscription(subscription);

            Assert.AreEqual(false, result.IsValid);
            Assert.AreEqual(SubscriptionValidationResult.ReasonType.Expired, result.Reason);
        }

        [Test]
        public void ValidateSubscription_SubscriptionHasNoQuantity_ReturnsCorrectResult()
        {
            var subscription = new Subscription
            {
                Status = SubscriptionStatus.Active,
                Quantity = 0
            };

            var result = SubscriptionService.ValidateSubscription(subscription);

            Assert.AreEqual(false, result.IsValid);
            Assert.AreEqual(SubscriptionValidationResult.ReasonType.NoQuantity, result.Reason);
        }

        [Test]
        public void ValidateSubscription_SubscriptionIsValid_ReturnsCorrectResult()
        {
            var subscription = GetValidSubscription();

            var result = SubscriptionService.ValidateSubscription(subscription);

            Assert.AreEqual(true, result.IsValid);
        }

        private static Subscription GetValidSubscription()
        {
            return new Subscription
            {
                Status = SubscriptionStatus.Active,
                Quantity = int.MaxValue,
                ExpirationUtc = DateTime.MaxValue
            };
        }

        [Test]
        public void BuySubscriptionWithTokens_SubscriptionPriceTokensIsNull_ThrowsException()
        {
            var subscription = GetValidSubscription();
            subscription.PriceTokens = null;

            var subscriptionService = new SubscriptionService(new InMemoryDataContextFactory());

            TestDelegate d = async () => await subscriptionService.BuySubscriptionWithTokens(subscription, null);

            Assert.Throws<Exception>(d);
        }

        [Test]
        public void BuySubscriptionWithTokens_UserHasInsufficientTokens_ThrowsException()
        {
            var subscription = GetValidSubscription();
            subscription.PriceTokens = 1;

            var user = new ApplicationUser
            {
                Tokens = 0
            };

            var subscriptionService = new SubscriptionService(new InMemoryDataContextFactory());

            TestDelegate d = async () => await subscriptionService.BuySubscriptionWithTokens(subscription, user);

            Assert.Throws<Exception>(d);
        }
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore PossibleNullReferenceException
}