using System;
using Mzayad.Models;
using Mzayad.Models.Enum;
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
            Assert.AreEqual(SubscriptionValidityResult.ReasonType.Null, result.Reason);
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
            Assert.AreEqual(SubscriptionValidityResult.ReasonType.Disabled, result.Reason);
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
            Assert.AreEqual(SubscriptionValidityResult.ReasonType.Expired, result.Reason);
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
            Assert.AreEqual(SubscriptionValidityResult.ReasonType.NoQuantity, result.Reason);
        }

        [Test]
        public void ValidateSubscription_SubscriptionIsValid_ReturnsCorrectResult()
        {
            var subscription = new Subscription
            {
                Status = SubscriptionStatus.Active,
                Quantity = int.MaxValue,
                ExpirationUtc = DateTime.MaxValue
            };

            var result = SubscriptionService.ValidateSubscription(subscription);

            Assert.AreEqual(true, result.IsValid);
        }
        
        [Test]
        public void BuySubscriptionWithTokens_SubscriptionIsExpired_ReturnsErrorResult()
        {
            
        }
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore PossibleNullReferenceException
}