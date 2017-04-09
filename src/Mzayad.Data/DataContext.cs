using Microsoft.AspNet.Identity.EntityFramework;
using Mzayad.Models;
using Mzayad.Models.Payment;
using OrangeJetpack.Base.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Data
{
    public class DataContext : IdentityDbContext<ApplicationUser>, IDataContext
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
        public IDbSet<Banner> Banners { get; set; }
        public DataContext() : base("DefaultConnection")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public static DataContext Create()
        {
            return new DataContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<decimal>().Configure(prop => prop.HasPrecision(18, 3));

            base.OnModelCreating(modelBuilder);
        }

        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }
        public override int SaveChanges()
        {
            SetModifiedUtc();

            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errors = TraceValidationErrors(ex);
                var message = $"EntityValidationErrors - {string.Join(",", errors)}";

                throw new Exception(message);
            }
        }

        public override Task<int> SaveChangesAsync()
        {
            SetModifiedUtc();

            try
            {
                return base.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                var errors = TraceValidationErrors(ex);
                var message = $"EntityValidationErrors - {string.Join(",", errors)}";

                throw new Exception(message);
            }
        }

        private void SetModifiedUtc()
        {
            var entities = ChangeTracker
                .Entries()
                .Where(i => i.State == EntityState.Modified)
                .Select(i => i.Entity as EntityBase);

            foreach (var entity in entities.Where(i => i != null))
            {
                entity.ModifiedUtc = DateTime.UtcNow;
            }
        }

        private static IEnumerable<string> TraceValidationErrors(DbEntityValidationException ex)
        {
            foreach (var validationErrors in ex.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    var errorMessage = $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}";

                    Trace.TraceError(errorMessage);

                    yield return errorMessage;
                }
            }
        }

        public Bid SubmitUserBid(int auctionId, int secondsLeft, string userId)
        {
            var userIdParam = GetParam("@UserId", SqlDbType.NVarChar);
            userIdParam.Direction = ParameterDirection.InputOutput;
            userIdParam.Value = userId;

            var bidIdParam = GetParam("@BidId", SqlDbType.Int);
            var amountParam = GetParam("@Amount", SqlDbType.Float);
            var userNameParam = GetParam("@UserName", SqlDbType.NVarChar);
            var avatarUrlParam = GetParam("@AvatarUrl", SqlDbType.NVarChar);

            Database.ExecuteSqlCommand("SubmitUserBid @AuctionId, @SecondsLeft, @UserId out, @BidId out, @Amount out, @UserName out, @AvatarUrl out",
                new SqlParameter("@AuctionId", auctionId),
                new SqlParameter("@SecondsLeft", secondsLeft),
                userIdParam, bidIdParam, amountParam, userNameParam, avatarUrlParam);

            Trace.TraceWarning($"SubmitUserBid, BidId: {bidIdParam.Value}");

            if (bidIdParam.Value == DBNull.Value)
            {
                return null;
            }

            return new Bid
            {
                BidId = Convert.ToInt32(bidIdParam.Value),
                Amount = Convert.ToDecimal(amountParam.Value),
                UserId = Convert.ToString(userIdParam.Value),
                User = new ApplicationUser
                {
                    Id = Convert.ToString(userIdParam),
                    UserName = Convert.ToString(userNameParam.Value),
                    AvatarUrl = Convert.ToString(avatarUrlParam.Value)
                }
            };
        }

        public Bid SubmitAutoBid(int auctionId, int secondsLeft)
        {
            var userIdParam = GetParam("@UserId", SqlDbType.NVarChar);
            var bidIdParam = GetParam("@BidId", SqlDbType.Int);
            var amountParam = GetParam("@Amount", SqlDbType.Float);
            var userNameParam = GetParam("@UserName", SqlDbType.NVarChar);
            var avatarUrlParam = GetParam("@AvatarUrl", SqlDbType.NVarChar);

            Database.ExecuteSqlCommand("SubmitAutoBid @AuctionId, @SecondsLeft, @UserId out, @BidId out, @Amount out, @UserName out, @AvatarUrl out",
                new SqlParameter("@AuctionId", auctionId),
                new SqlParameter("@SecondsLeft", secondsLeft),
                userIdParam, bidIdParam, amountParam, userNameParam, avatarUrlParam);

            Trace.TraceWarning($"SubmitAutoBid, BidId: {bidIdParam.Value}");

            if (bidIdParam.Value == DBNull.Value)
            {
                return null;
            }

            return new Bid
            {
                BidId = Convert.ToInt32(bidIdParam.Value),
                Amount = Convert.ToDecimal(amountParam.Value),
                UserId = Convert.ToString(userIdParam.Value),
                User = new ApplicationUser
                {
                    Id = Convert.ToString(userIdParam),
                    UserName = Convert.ToString(userNameParam.Value),
                    AvatarUrl = Convert.ToString(avatarUrlParam.Value)
                }
            };
        }

        private static SqlParameter GetParam(string name, SqlDbType type)
        {
            var sqlParameter = new SqlParameter
            {
                ParameterName = name,
                Direction = ParameterDirection.Output,
                SqlDbType = type
            };

            if (type == SqlDbType.NVarChar)
            {
                sqlParameter.Size = -1;
            }

            return sqlParameter;
        }
    }
}
