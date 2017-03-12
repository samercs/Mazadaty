using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Payment;
using System.Data.Entity;
using System.Threading.Tasks;

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
        public IDbSet<ShippingAddress> ShippingAddresses { get; set; }
        public IDbSet<Order> Orders { get; set; }
        public IDbSet<OrderItem> OrderItems { get; set; }
        public IDbSet<OrderLog> OrderLogs { get; set; }
        public IDbSet<Subscription> Subscriptions { get; set; }
        public IDbSet<WishList> WishLists { get; set; }
        public IDbSet<KnetTransaction> KnetTransactions { get; set; }
        public IDbSet<SubscriptionLog> SubscriptionLogs { get; set; }
        public IDbSet<TokenLog> TokenLogs { get; set; }
        public IDbSet<SplashAd> SplashAds { get; set; }
        public IDbSet<ActivityEvent> ActivityEvents { get; set; }
        public IDbSet<Trophy> Trophies { get; set; }
        public IDbSet<UserTrophy> UsersTrophies { get; set; }
        public IDbSet<IslamicCalendar> IslamicCalendars { get; set; }
        public IDbSet<SessionLog> SessionLogs { get; set; }
        public IDbSet<AutoBid> AutoBids { get; set; }
        public IDbSet<Prize> Prizes { get; set; }
        public IDbSet<UserPrizeLog> UserPrizeLogs { get; set; }
        public IDbSet<UserAvatar> UserAvatars { get; set; }
        public IDbSet<UserFriend> UsersFriends { get; set; }
        public IDbSet<FriendRequest> FriendsRequests { get; set; }
        public IDbSet<Message> Messages { get; set; }

        public InMemoryDataContext()
        {
            Auctions = new TestDbSet<Auction>();
            Categories = new TestDbSet<Category>();
            CategoryNotifications = new TestDbSet<CategoryNotification>();
            Orders = new TestDbSet<Order>();
            Bids = new TestDbSet<Bid>();
            AutoBids = new TestDbSet<AutoBid>();
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

        public Bid SubmitUserBid(int auctionId, int secondsLeft, string userId)
        {
            throw new System.NotImplementedException();
        }

        public Bid SubmitAutoBid(int auctionId, int secondsLeft)
        {
            throw new System.NotImplementedException();
        }
    }
}
