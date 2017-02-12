using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Models.Enums;

namespace Mzayad.Services
{
    public class OrderService : ServiceBase
    {
        public OrderService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<Order> GetById(int id, string languageCode = "en")
        {
            using (var dc = DataContext())
            {
                var order = await dc.Orders
                    .Include(i => i.Address)
                    .Include(i => i.Items.Select(j => j.Product))
                    .Include(i => i.Items.Select(j => j.Subscription))
                    .Include(i => i.Logs)
                    .SingleOrDefaultAsync(i => i.OrderId == id);

                if (order == null)
                {
                    return null;
                }

                foreach (var item in order.Items)
                {
                    item.Localize(languageCode, i => i.Name);
                }

                return order;
            }
        }

        public async Task<IReadOnlyCollection<Order>> GetOrders(OrderStatus status, string search = "")
        {
            using (var dc = DataContext())
            {
                var query = dc.Orders.Where(i => i.Status == status);
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(i =>
                        i.Address.Name.Contains(search) ||
                        i.Address.PhoneLocalNumber.Contains(search));
                }

                return await query
                    .Include(i => i.Address)
                    .Include(i => i.Items)
                    .OrderBy(i => i.SubmittedUtc)
                    .ToListAsync();
            }
        }

        public async Task<Order> CreateOrderForAuction(Auction auction, Bid bid)
        {
            using (var dc = DataContext())
            {
                var order = new Order
                {
                    Type = OrderType.Auction,
                    UserId = bid.UserId,
                    Status = OrderStatus.InProgress,
                    PaymentMethod = PaymentMethod.Knet,
                    AllowPhoneSms = false,
                    Address = ShippingAddress.Create(bid.User),
                    Items = CreateOrderItems(auction, bid.Amount),
                    Logs = CreateOrderLogs(bid.UserId)
                };

                dc.Orders.Add(order);
                await dc.SaveChangesAsync();
                return order;
            }
        }

        public async Task<Order> CreateOrderForBuyNow(Auction auction, ApplicationUser user)
        {
            using (var dc = DataContext())
            {
                var order = new Order
                {
                    Type = OrderType.Auction,
                    UserId = user.Id,
                    Status = OrderStatus.InProgress,
                    PaymentMethod = PaymentMethod.Knet,
                    AllowPhoneSms = false,
                    Address = ShippingAddress.Create(user),
                    Items = CreateOrderItems(auction, auction.BuyNowPrice),
                    Logs = CreateOrderLogs(user.Id)
                };

                dc.Orders.Add(order);
                await dc.SaveChangesAsync();
                return order;
            }
        }

        public async Task<Order> CreateOrderForSubscription(Subscription subscription, ApplicationUser user, PaymentMethod paymentMethod)
        {
            using (var dc = DataContext())
            {
                var order = new Order
                {
                    Type = OrderType.Subscription,
                    UserId = user.Id,
                    AllowPhoneSms = false,
                    Address = ShippingAddress.Create(user)
                };

                SetStatus(order, OrderStatus.InProgress, user.Id);

                switch (paymentMethod)
                {
                    case PaymentMethod.Tokens:
                        UpdateOrderForTokenPayment(order, subscription);
                        break;
                    case PaymentMethod.Knet:
                        UpdateOrderForKnetPayment(order, subscription);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                order.RecalculateTotal();

                dc.Orders.Add(order);
                dc.Subscriptions.Attach(subscription);

                await dc.SaveChangesAsync();
                return order;
            }
        }

        public async Task<Order> CreateOrder(Order order)
        {
            using (var dc= DataContext())
            {
                dc.Orders.Add(order);
                SetStatus(order, OrderStatus.PendingPayment, order.UserId);
                await dc.SaveChangesAsync();
                return order;
            }
        }

        private static void UpdateOrderForTokenPayment(Order order, Subscription subscription)
        {
            UpdateOrderForPayment(order, subscription, PaymentMethod.Tokens, 0);
        }

        private static void UpdateOrderForKnetPayment(Order order, Subscription subscription)
        {
            UpdateOrderForPayment(order, subscription, PaymentMethod.Knet, subscription.PriceCurrency.GetValueOrDefault(0));
        }

        private static void UpdateOrderForPayment(Order order, Subscription subscription, PaymentMethod paymentMethod, decimal itemPrice)
        {
            order.PaymentMethod = paymentMethod;
            order.Items = new[]
            {
                new OrderItem
                {
                    ItemPrice = itemPrice,
                    Name = subscription.Name,
                    Quantity = 1,
                    SubscriptionId = subscription.SubscriptionId,
                    Subscription = subscription
                }
            };

            order.SubmittedUtc = DateTime.UtcNow;
        }

        private static OrderItem[] CreateOrderItems(Auction auction, decimal? itemPrice)
        {
            if (!itemPrice.HasValue)
            {
                throw new Exception("Cannot create order with no item price.");
            }

            return new[]
            {
                new OrderItem
                {
                    ItemPrice = itemPrice.Value,
                    Name = auction.Product.Name,
                    Quantity = 1,
                    ProductId = auction.ProductId
                }
            };
        }

        private static OrderLog[] CreateOrderLogs(string userId)
        {
            return new[]
            {
                new OrderLog
                {
                    Status = OrderStatus.InProgress,
                    UserId = userId
                }
            };
        }

        public async Task<Order> Update(Order order)
        {
            using (var dc = DataContext())
            {
                dc.Orders.Attach(order);
                dc.ShippingAddresses.Attach(order.Address);
                dc.SetModified(order);
                dc.SetModified(order.Address);
                await dc.SaveChangesAsync();
                return order;
            }
        }

        /// <summary>
        /// Updates an order status and adds a log of the status change.
        /// </summary>
        public async Task<Order> UpdateStatus(Order order, OrderStatus status, string modifiedUserId)
        {
            using (var dc = DataContext())
            {
                dc.Orders.Attach(order);

                SetStatus(order, status, modifiedUserId);

                await dc.SaveChangesAsync();

                return order;
            }
        }

        private static void SetStatus(Order order, OrderStatus status, string modifiedByUserId)
        {
            order.Status = status;
            order.Logs = order.Logs ?? new HashSet<OrderLog>();
            order.Logs.Add(new OrderLog
            {
                UserId = modifiedByUserId,
                Status = status
            });
        }

        public async Task<Order> SaveShippingAndPayment(Order order, PaymentMethod? paymentMethod)
        {
            using (var dc = DataContext())
            {
                dc.Orders.Attach(order);

                order.RecalculateTotal();
                order.PaymentMethod = paymentMethod;
                order.SubmittedUtc = DateTime.UtcNow;

                await dc.SaveChangesAsync();

                return order;
            }
        }

        public async Task SubmitOrderForProcessing(Order order, string modifiedByUserId)
        {
            using (var dc = DataContext())
            {
                dc.Orders.Attach(order);

                DecrementProductOrSubscriptionInventory(order);

                SetStatus(order, OrderStatus.Processing, modifiedByUserId);

                await dc.SaveChangesAsync();
            }
        }

        public async Task SaveOrderAsDelivered(Order order, string modifiedByUserId)
        {
            using (var dc = DataContext())
            {
                dc.Orders.Attach(order);
                SetStatus(order, OrderStatus.Delivered, modifiedByUserId);
                order.ShippedUtc = DateTime.UtcNow;
                await dc.SaveChangesAsync();
            }
        }

        public async Task CompleteSubscriptionOrder(Order order, string modifiedByUserId, string userHostAddress)
        {
            var subscriptionService = new SubscriptionService(DataContextFactory);

            var subscriptionItems = order.Items.Where(orderItem => orderItem.SubscriptionId.HasValue);
            foreach (var item in subscriptionItems)
            {
                await subscriptionService.AddSubscriptionToUser(order.UserId, item.Subscription, modifiedByUserId, userHostAddress);
            }

            await SaveOrderAsDelivered(order, modifiedByUserId);
        }

        public async Task<IReadOnlyCollection<BuyNowTransactionModel>> GetBuyNowTransactions(DateTime startDate, DateTime endDate)
        {
            using (var dc = DataContext())
            {
                var orders = await dc.OrderItems
                    .Include(i => i.Order.User)
                    .Include(i => i.Product)
                    .Where(i => i.Order.Status == OrderStatus.Delivered)
                    .Where(i => i.Order.Type == OrderType.BuyNow)
                    .Where(i => i.Order.CreatedUtc >= startDate)
                    .Where(i => i.Order.CreatedUtc <= endDate)
                    .ToListAsync();

                return orders.Select(BuyNowTransactionModel.Create).ToList();

            }
        }

        private static void DecrementProductOrSubscriptionInventory(Order order)
        {
            foreach (var item in order.Items)
            {
                if (item.Product != null)
                {
                    item.Product.Quantity -= 1;
                }

                if (item.Subscription != null)
                {
                    item.Subscription.Quantity -= 1;
                }
            }
        }
    }
}
