using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Mzayad.Models;
using Mzayad.Models.Payment;

namespace Mzayad.Data
{
    public interface IDataContext : IDisposable
    {
        IDbSet<Address> Addresses { get; set; }
        IDbSet<EmailTemplate> EmailTemplates { get; set; }
        IDbSet<Category> Categories { get; set; }
        IDbSet<Product> Products { get; set; }
        IDbSet<ProductImage> ProductImages { get; set; }
        IDbSet<Auction> Auctions { get; set; }
        IDbSet<Specification> Specifications { get; set; }
        IDbSet<CategoryNotification> CategoryNotifications { get; set; }
        IDbSet<Sponsor> Sponsors { get; set; }
        IDbSet<Bid> Bids { get; set; }
        IDbSet<Avatar> Avatars { get; set; }
        IDbSet<UserProfile> UserProfiles { get; set; }
        IDbSet<ShippingAddress> ShippingAddresses { get; set; }
        IDbSet<Order> Orders { get; set; }
        IDbSet<OrderItem> OrderItems { get; set; }
        IDbSet<OrderLog> OrderLogs { get; set; }
        IDbSet<Subscription> Subscriptions { get; set; }
        IDbSet<WishList> WishLists { get; set; }
        IDbSet<KnetTransaction> KnetTransactions { get; set; }
        IDbSet<SubscriptionLog> SubscriptionLogs { get; set; } 
        IDbSet<SplashAd> SplashAds { get; set; } 
        
        int SaveChanges();
        Task<int> SaveChangesAsync();
        void SetModified(object entity);
    }
}
