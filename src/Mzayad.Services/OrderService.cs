using System.Collections.Generic;
using System.Linq;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using OrangeJetpack.Base.Core.Formatting;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class OrderService : ServiceBase
    {
        public OrderService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<Order> GetById(int id)
        {
            using (var dc = DataContext())
            {
                return await dc.Orders
                    .Include(i => i.Address)
                    .Include(i => i.Items)
                    .Include(i => i.Logs)
                    .SingleOrDefaultAsync(i => i.OrderId == id);
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

        public async Task<Order> CreateOrder(int auctionId, int bidId)
        {
            using (var dc = DataContext())
            {
                var bid = await dc.Bids.Include(i => i.User.Address).SingleOrDefaultAsync(i => i.BidId == bidId);
                
                var auction =
                    await dc.Auctions.Include(i => i.Product).SingleOrDefaultAsync(i => i.AuctionId == auctionId);

                if (bid == null || auction == null)
                {
                    return null;
                }
                
                var order = CreateOrder(auction, bid);

                dc.Orders.Add(order);
                await dc.SaveChangesAsync();
                return order;
            }
        }

        private static Order CreateOrder(Auction auction, Bid bid)
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

        private static ShippingAddress CreateShippingAddress(Bid bid)
        {
            return new ShippingAddress
            {
                AddressLine1 = bid.User.Address.AddressLine1,
                AddressLine2 = bid.User.Address.AddressLine2,
                AddressLine3 = bid.User.Address.AddressLine3,
                AddressLine4 = bid.User.Address.AddressLine4,
                CityArea = bid.User.Address.CityArea,
                CountryCode = bid.User.Address.CountryCode,
                PostalCode = bid.User.Address.PostalCode,
                StateProvince = bid.User.Address.StateProvince,
                            
                Name = NameFormatter.GetFullName(bid.User.FirstName, bid.User.LastName),
                PhoneCountryCode = bid.User.PhoneCountryCode,
                PhoneLocalNumber = bid.User.PhoneNumber               
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
            using (var dc=DataContext())
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
    }
}
