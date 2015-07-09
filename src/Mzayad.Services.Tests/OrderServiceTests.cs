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
    public class OrderServiceTests
    {
        [Test]
        public async Task CreateOrderForSubscription_PaymentMethodIsTokens_StatusIsShipped()
        {
            var subscription = new Subscription
            {
                Status = SubscriptionStatus.Active
            };

            var user = new ApplicationUser
            {
                Id = Constants.AnyUserId,
                Address = new Address()
            };
            
            var orderService = new OrderService(new InMemoryDataContextFactory());

            var order = await orderService.CreateOrderForSubscription(subscription, user, PaymentMethod.Tokens);

            Assert.AreEqual(OrderStatus.Shipped, order.Status);
        }
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore PossibleNullReferenceException
}