using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Order>> GetOrders(OrderStatus status, string search = "")
        {
            using (var dc = DataContext())
            {
                var query = dc.Orders.Where(i => i.Status == status);
                if (!string.IsNullOrEmpty(search))
                {
                    query =
                        query.Where(i => i.Address.Name.Contains(search) || i.Address.PhoneLocalNumber.Contains(search));

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
                    Items = CreateOrderItems(bid, auction),
                    Logs = CreateOrderLogs(bid)
                };

                dc.Orders.Add(order);
                await dc.SaveChangesAsync();
                return order;
            }
        }

        public async Task<Order> CreateOrderForSubscription(Subscription subscription, ApplicationUser user, PaymentMethod paymentMethod, string userHostAddress)
        {          
            var order = new Order
            {
                Type = OrderType.Subscription,
                UserId = user.Id,
                AllowPhoneSms = false,
                Address = ShippingAddress.Create(user)
            };

            SetStatus(order, OrderStatus.InProgress, user.Id, userHostAddress);

            switch (paymentMethod)
            {
                case PaymentMethod.Tokens:
                    UpdateOrderForTokenPayment(order, subscription, userHostAddress);
                    break;
                case PaymentMethod.Knet:
                    UpdateOrderForKnetPayment(order, subscription, userHostAddress);
                    break;
                default:
                    throw new NotImplementedException();
            }

            order.RecalculateTotal();

            using (var dc = DataContext())
            {
                dc.Orders.Add(order);
                await dc.SaveChangesAsync();
                return order;
            }
        }

        private static void UpdateOrderForTokenPayment(Order order, Subscription subscription, string userHostAddress)
        {
            order.PaymentMethod = PaymentMethod.Tokens;
            order.Items = new[]
            {
                new OrderItem
                {
                    ItemPrice = 0,
                    Name = subscription.Name,
                    Quantity = 1,
                    SubscriptionId = subscription.SubscriptionId
                }
            };

            // purchases with tokens are automaticallyed "shipped"
            SetStatus(order, OrderStatus.Shipped, order.UserId, userHostAddress);

            var utcNow = DateTime.UtcNow;

            order.SubmittedUtc = utcNow;
            order.ShippedUtc = utcNow;
        }

        private static void UpdateOrderForKnetPayment(Order order, Subscription subscription, string userHostAddress)
        {
            order.PaymentMethod = PaymentMethod.Tokens;
            order.Items = new[]
            {
                new OrderItem
                {
                    ItemPrice = subscription.PriceCurrency.GetValueOrDefault(0),
                    Name = subscription.Name,
                    Quantity = 1,
                    SubscriptionId = subscription.SubscriptionId
                }
            };

            order.SubmittedUtc = DateTime.UtcNow;
        }

        private static OrderItem[] CreateOrderItems(Bid bid, Auction auction)
        {
            return new[]
            {
                new OrderItem
                {
                    ItemPrice = bid.Amount,
                    Name = auction.Product.Name,
                    Quantity = 1,
                    ProductId = auction.ProductId
                }
            };
        }

        private static OrderLog[] CreateOrderLogs(Bid bid)
        {
            return new[]
            {
                new OrderLog
                {
                    Status = OrderStatus.InProgress,
                    UserId = bid.UserId,
                    UserHostAddress = bid.UserHostAddress
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
        /// Updates an order status and addes a log of the status change.
        /// </summary>
        public async Task<Order> UpdateStatus(Order order, OrderStatus status, string userHostAddress, string userId = null)
        {
            using (var dc = DataContext())
            {
                dc.Orders.Attach(order);

                SetStatus(order, status, userId, userHostAddress);

                await dc.SaveChangesAsync();

                return order;
            }
        }

        private static void SetStatus(Order order, OrderStatus status, string userId, string userHostAddress)
        {
            order.Status = status;
            order.Logs = order.Logs ?? new HashSet<OrderLog>();
            order.Logs.Add(new OrderLog
            {
                UserId = userId,
                UserHostAddress = userHostAddress,
                Status = status
            });
        }

        public async Task SetStatusAsShipped(Order order,  string userId, string userHostAddress)
        {
            using (var dc = DataContext())
            {
                dc.Orders.Attach(order);
                SetStatus(order, OrderStatus.Shipped, userId, userHostAddress);
                order.ShippedUtc = DateTime.UtcNow;
                await dc.SaveChangesAsync();
            }
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

        public async Task SubmitOrder(Order order, string userHostAddress)
        {
            using (var dc = DataContext())
            {
                dc.Orders.Attach(order);

                DecrementProductOrSubscriptionInventory(order);

                var orderHasPhysicalGoods = order.Items.Any(i => i.ProductId.HasValue);
                if (orderHasPhysicalGoods)
                {
                    SetStatus(order, OrderStatus.Processing, null, userHostAddress);
                }
                else
                {
                    SetStatus(order, OrderStatus.Shipped, null, userHostAddress);
                    order.ShippedUtc = DateTime.UtcNow;
                }

                await dc.SaveChangesAsync();
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
