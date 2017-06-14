using Mzayad.Models;
using Mzayad.Models.Payment;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

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
        IDbSet<ShippingAddress> ShippingAddresses { get; set; }
        IDbSet<Order> Orders { get; set; }
        IDbSet<OrderItem> OrderItems { get; set; }
        IDbSet<OrderLog> OrderLogs { get; set; }
        IDbSet<Subscription> Subscriptions { get; set; }
        IDbSet<WishList> WishLists { get; set; }
        IDbSet<KnetTransaction> KnetTransactions { get; set; }
        IDbSet<SubscriptionLog> SubscriptionLogs { get; set; }
        IDbSet<TokenLog> TokenLogs { get; set; }
        IDbSet<SplashAd> SplashAds { get; set; }
        IDbSet<ActivityEvent> ActivityEvents { get; set; }
        IDbSet<Trophy> Trophies { get; set; }
        IDbSet<UserTrophy> UsersTrophies { get; set; }
        IDbSet<IslamicCalendar> IslamicCalendars { get; set; }
        IDbSet<SessionLog> SessionLogs { get; set; }
        IDbSet<AutoBid> AutoBids { get; set; }
        IDbSet<Prize> Prizes { get; set; }
        IDbSet<UserPrizeLog> UserPrizeLogs { get; set; }
        IDbSet<UserAvatar> UserAvatars { get; set; }
        IDbSet<UserFriend> UsersFriends { get; set; }
        IDbSet<FriendRequest> FriendsRequests { get; set; }
        IDbSet<Message> Messages { get; set; }
        IDbSet<Banner> Banners { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
        void SetModified(object entity);

        Bid SubmitUserBid(int auctionId, int secondsLeft, string userId);
        Bid SubmitAutoBid(int auctionId, int secondsLeft);
    }
}
