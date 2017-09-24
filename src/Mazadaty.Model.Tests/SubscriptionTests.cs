using Mazadaty.Models;
using NUnit.Framework;

namespace Mazadaty.Model.Tests
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable PossibleNullReferenceException

    [TestFixture]
    public class SubscriptionTests
    {
        [Test]
        public void Subscription_QuantityIsNull_IsSoldOutIsFalse()
        {
            var subscription = new Subscription
            {
                Quantity = null
            };

            Assert.IsFalse(subscription.IsSoldOut);
        }

        [TestCase(-1, true)]
        [TestCase(0, true)]
        [TestCase(1, false)]
        public void Subscription_QuantityVaries_IsSoldOutIsCorrect(int quantity, bool expected)
        {
            var subscription = new Subscription
            {
                Quantity = quantity
            };

            Assert.AreEqual(expected, subscription.IsSoldOut);
        }
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore PossibleNullReferenceException
}
