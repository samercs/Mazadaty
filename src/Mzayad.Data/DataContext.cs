using Microsoft.AspNet.Identity.EntityFramework;
using Mzayad.Models;
using Mzayad.Models.Payment;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OrangeJetpack.Base.Data;

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
        public IDbSet<Trophy> Trophies{ get; set; }
        public IDbSet<UserTrophy> UsersTrophies { get; set; }
        public IDbSet<IslamicCalendar> IslamicCalendars { get; set; }
        public IDbSet<SessionLog> SessionLogs { get; set; }
        public IDbSet<AutoBid> AutoBids { get; set; }

        public DataContext(): base("DefaultConnection")
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
                var message = string.Format("EntityValidationErrors - {0}", string.Join(",", errors));

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
                    var errorMessage = string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                    Trace.TraceError(errorMessage);

                    yield return errorMessage;
                }
            }
        }
    }
}
