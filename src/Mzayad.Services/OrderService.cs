using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Core.Formatting;
using Mzayad.Data;
using Mzayad.Data.Migrations;
using Mzayad.Models;
using Mzayad.Models.Enum;

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
                            
                            Email = bid.User.Email,
                            FirstName = bid.User.FirstName,
                            LastName = bid.User.LastName,
                            PhoneNumber = PhoneNumberFormatter.Format(bid.User.PhoneCountryCode,bid.User.PhoneNumber)
                            
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
    }
}
