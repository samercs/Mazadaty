using System.Data.Entity;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Payment;

namespace Mzayad.Services.Tests.Fakes
{
    public class InMemoryDataContext : IDataContext
    {
        public IDbSet<Address> Addresses { get; set; }
        public IDbSet<EmailTemplate> EmailTemplates { get; set; }
        public IDbSet<Category> Categories { get; set; }
        public IDbSet<Product> Products { get; set; }
        public IDbSet<ProductImage> ProductImages { get; set; }
        public IDbSet<Auction> Auctions { get; set; }
        public IDbSet<Specification> Specifications { get; set; }
        public IDbSet<CategoryNotification> CategoryNotifications { get; set; }
        public IDbSet<Sponsor> Sponsors { get; set; }
        public IDbSet<Bid> Bids { get; set; }
        public IDbSet<Avatar> Avatars { get; set; }
        public IDbSet<UserProfile> UserProfiles { get; set; }
        public IDbSet<ShippingAddress> ShippingAddresses { get; set; }
        public IDbSet<Order> Orders { get; set; }
        public IDbSet<OrderItem> OrderItems { get; set; }
        public IDbSet<OrderLog> OrderLogs { get; set; }
        public IDbSet<Subscription> Subscriptions { get; set; }
        public IDbSet<WishList> WishLists { get; set; }
        public IDbSet<KnetTransaction> KnetTransactions { get; set; }

        public InMemoryDataContext()
        {
            Auctions = new TestDbSet<Auction>();
            Categories = new TestDbSet<Category>();
            CategoryNotifications = new TestDbSet<CategoryNotification>();
        }

        public int SaveChangesCount { get; private set; }
        public int SaveChanges()
        {
            SaveChangesCount++;
            return 1;
        }

        public Task<int> SaveChangesAsync()
        {
            SaveChangesCount++;
            return Task<int>.Factory.StartNew(() => 1);
        }
        
        public void SetModified(object entity)
        {
            // do nothing
        }

        public void Dispose()
        {
            // do nothing
        }
    }
}
