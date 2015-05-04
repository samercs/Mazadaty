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

        public async Task<Order> CreateOrder(int auctionId,int bidId)
        {
            using (var dc = DataContext())
            {

                var bid = await dc.Bids.Include(i => i.User.Address ).SingleOrDefaultAsync(i => i.BidId == bidId);
                
                var auction =
                    await dc.Auctions.Include(i => i.Product).SingleOrDefaultAsync(i => i.AuctionId == auctionId);
                if (bid != null && auction!=null)
                {
                    var order = new Order()
                    {
                        PaymentMethod = PaymentMethod.Knet,
                        Status = OrderStatus.InProgress,
                        UserId = bid.UserId,
                        AllowPhoneSms = false,
                        Address = new ShippingAddress()
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
                        },
                        Type = OrderType.Auction,
                        SubmittedUtc = DateTime.UtcNow,
                        Items = new[]
                        {
                            new OrderItem()
                            {
                                ItemPrice = bid.Amount,
                                Name = auction.Product.Name,
                                Quantity = auction.Product.Quantity,
                                ProductId = auction.ProductId
                            }
                        },
                        Logs = new[]
                        {
                            new OrderLog()
                            {
                                Status = OrderStatus.InProgress,
                                UserId = bid.UserId,
                                UserHostAddress = bid.UserHostAddress

                            }
                        }


                    };

                    dc.Orders.Add(order);
                    await dc.SaveChangesAsync();
                    return order;
                }
                return null;
            }
        }

        public async Task<Order> GetById(int id)
        {
            using (var dc=DataContext())
            {
                return await 
                    dc.Orders.Include(i => i.Address)
                        .Include(i => i.Items)
                        .Include(i => i.Logs)
                        .SingleOrDefaultAsync(i => i.OrderId == id);
            }
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


        public async  Task<IEnumerable<Order>> GetOrders(OrderStatus status, string search="")
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

        public async Task<Order> GetOrder(int id)
        {
            using (var dc = DataContext())
            {
                return await dc.Orders.Include(i => i.Items).Include(i => i.Address).FirstOrDefaultAsync(i => i.OrderId == id);
            }
        }
    }
}
