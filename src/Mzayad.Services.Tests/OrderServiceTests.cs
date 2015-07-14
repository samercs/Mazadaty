﻿using System.Linq;
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
        private static async Task<Order> CreateOrderForSubscription()
        {
            var subscription = new Subscription
            {
                SubscriptionId = Constants.AnyInt,
                Status = SubscriptionStatus.Active
            };

            var user = new ApplicationUser
            {
                Id = Constants.AnyUserId,
                Address = new Address()
            };

            var orderService = new OrderService(new InMemoryDataContextFactory());

            return await orderService.CreateOrderForSubscription(subscription, user, PaymentMethod.Tokens, Constants.AnyHostAddress);
        }
        
        [Test]
        public async Task CreateOrderForSubscription_PaymentMethodIsTokens_EnumsSetCorrectly()
        {
            var order = await CreateOrderForSubscription();

            Assert.AreEqual(OrderType.Subscription, order.Type);
            Assert.AreEqual(PaymentMethod.Tokens, order.PaymentMethod);
            Assert.AreEqual(OrderStatus.Shipped, order.Status);
        }

        [Test]
        public async Task CreateOrderForSubscription_PaymentMethodIsTokens_HasCorrectOrderItems()
        {
            var order = await CreateOrderForSubscription();

            Assert.AreEqual(1, order.Items.Count);
        }

        [Test]
        public async Task CreateOrderForSubscription_PaymentMethodIsTokens_HasCorrectOrderLogs()
        {
            var order = await CreateOrderForSubscription();

            Assert.AreEqual(2, order.Logs.Count);

            CollectionAssert.Contains(order.Logs.Select(i => i.Status), OrderStatus.InProgress);
            CollectionAssert.Contains(order.Logs.Select(i => i.Status), OrderStatus.Shipped);
        }

    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore PossibleNullReferenceException
}