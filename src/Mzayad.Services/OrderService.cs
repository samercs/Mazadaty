using System.Collections.Generic;
using System.Linq;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using OrangeJetpack.Base.Core.Formatting;
using System;
using System.Data.Entity;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using OrangeJetpack.Localization;

namespace Mzayad.Services
{
    public class OrderService : ServiceBase
    {
        private readonly SubscriptionService _subscriptionService;
        
        public OrderService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
            _subscriptionService = new SubscriptionService(dataContextFactory);
        }

        public async Task<Order> GetById(int id, string languageCode = "en")
        {
            using (var dc = DataContext())
            {
                var order = await dc.Orders
                    .Include(i => i.Address)
                    .Include(i => i.Items)
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


                return
                    await query.Include(i => i.Address).Include(i => i.Items).OrderBy(i => i.SubmittedUtc).ToListAsync();
            }
        }

        public async Task<Order> CreateOrderForAuction(Auction auction, int bidId)
        {
            using (var dc = DataContext())
            {
                var bid = await dc.Bids.Include(i => i.User.Address).SingleOrDefaultAsync(i => i.BidId == bidId);
                if (bid == null)
                {
                    return null;
                }
                
                var order = CreateOrderForAuction(auction, bid);

                dc.Orders.Add(order);
                await dc.SaveChangesAsync();
                return order;
            }
        }

        private static Order CreateOrderForAuction(Auction auction, Bid bid)
        {
            return new Order
            {
                Type = OrderType.Auction,
                UserId = bid.UserId,
                Status = OrderStatus.InProgress,
                PaymentMethod = PaymentMethod.Knet,
                AllowPhoneSms = false,
                Address = CreateShippingAddress(bid),
                Items = CreateOrderItems(bid, auction),
                Logs = CreateOrderLogs(bid)
            };
        }

        public async Task<Order> CreateOrderForSubscription(Subscription subscription, ApplicationUser user, PaymentMethod paymentMethod)
        {
            if (!_subscriptionService.ValidateSubscription(subscription))
            {
                throw new Exception("Subscription is no longer valid.");
            }
            
            var order = new Order
            {
                Type = OrderType.Subscription,
                UserId = user.Id,
                AllowPhoneSms = false,
                Address = CreateShippingAddress(user)
            };

            switch (paymentMethod)
            {
                case PaymentMethod.Knet:
                    throw new NotImplementedException();
                case PaymentMethod.Tokens:
                    UpdateOrderForTokenPayment(order, subscription);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return order;
        }

        private static void UpdateOrderForTokenPayment(Order order, Subscription subscription)
        {
            order.PaymentMethod = PaymentMethod.Tokens;
            order.Status = OrderStatus.Shipped;
            order.Items = new[]
            {
                new OrderItem
                {
                    ItemPrice = 0,
                    Name = subscription.Name,
                    Quantity = 1,
                    //
                }
            };
        }

        private static ShippingAddress CreateShippingAddress(Bid bid)
        {
            return CreateShippingAddress(bid.User);
        }

        private static ShippingAddress CreateShippingAddress(ApplicationUser user)
        {
            return new ShippingAddress
            {
                AddressLine1 = user.Address.AddressLine1,
                AddressLine2 = user.Address.AddressLine2,
                AddressLine3 = user.Address.AddressLine3,
                AddressLine4 = user.Address.AddressLine4,
                CityArea = user.Address.CityArea,
                CountryCode = user.Address.CountryCode,
                PostalCode = user.Address.PostalCode,
                StateProvince = user.Address.StateProvince,

                Name = NameFormatter.GetFullName(user.FirstName, user.LastName),
                PhoneCountryCode = user.PhoneCountryCode,
                PhoneLocalNumber = user.PhoneNumber
            };
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
            //using (var dc = DataContext())
            //{
            //    foreach (var item in order.Items)
            //    {
            //        var skuId = item.SkuId;
            //        var sku = await dc.Skus.SingleOrDefaultAsync(i => i.SkuId == skuId);
            //        if (sku != null)
            //        {
            //            sku.Quantity--;
            //        }
            //    }
            //    await dc.SaveChangesAsync();
            //}

            await UpdateStatus(order, OrderStatus.Processing, userHostAddress);
        }
    }
}
