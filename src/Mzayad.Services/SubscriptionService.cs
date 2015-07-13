using System;
using Mzayad.Data;
using Mzayad.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Models.Enum;
using Mzayad.Services.Identity;
using OrangeJetpack.Localization;

namespace Mzayad.Services
{
    public class SubscriptionService : ServiceBase
    {
        private readonly UserManager  _userManager; 
        private readonly TokenService _tokenService;
        private readonly OrderService _orderService;
        
        public SubscriptionService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
            _userManager = new UserManager(dataContextFactory);
            _tokenService = new TokenService(dataContextFactory);
            _orderService = new OrderService(dataContextFactory);
        }

        public async Task<IReadOnlyCollection<Subscription>> GetActiveSubscriptions(string languageCode)
        {
            using (var dc = DataContext())
            {
                var subscriptions = await dc.Subscriptions
                    .Where(i => i.Status == SubscriptionStatus.Active)
                    .Where(i => !i.ExpirationUtc.HasValue || i.ExpirationUtc > DateTime.UtcNow)
                    .OrderBy(i => i.SortOrder)
                    .ThenBy(i => i.ExpirationUtc)
                    .ToListAsync();

                return subscriptions.Localize(languageCode, i => i.Name).ToList();
            }
        }

        public async Task<IReadOnlyCollection<Subscription>> GetAll(string languageCode = null)
        {
            using (var dc = DataContext())
            {
                var subscriptions = await dc.Subscriptions
                    .OrderBy(i => i.SortOrder)
                    .ThenBy(i => i.ExpirationUtc)
                    .ToListAsync();

                if (!string.IsNullOrWhiteSpace(languageCode))
                {
                    subscriptions = subscriptions.Localize(languageCode, i => i.Name).ToList();
                }

                return subscriptions;
            }
        }

        public async Task<Subscription> GetById(int subscriptionId)
        {
            using (var dc = DataContext())
            {
                return await dc.Subscriptions.SingleOrDefaultAsync(i => i.SubscriptionId == subscriptionId);
            }
        }

        /// <summary>
        /// Gets an active, non-expired, quantity available subscription.
        /// </summary>
        public async Task<Subscription> GetValidSubscription(int subscriptionId, string languageCode = null)
        {
            using (var dc = DataContext())
            {
                var subscription = await dc.Subscriptions
                    .Where(i => i.Status == SubscriptionStatus.Active)
                    .Where(i => !i.ExpirationUtc.HasValue || i.ExpirationUtc > DateTime.UtcNow)
                    .Where(i => !i.Quantity.HasValue || i.Quantity > 0)
                    .SingleOrDefaultAsync(i => i.SubscriptionId == subscriptionId);

                if (subscription != null && languageCode != null)
                {
                    subscription = subscription.Localize(languageCode, i => i.Name);
                }

                return subscription;
            }
        }

        public async Task<Subscription> Add(Subscription subscription)
        {
            using (var dc = DataContext())
            {
                dc.Subscriptions.Add(subscription);
                await dc.SaveChangesAsync();
                return subscription;
            }
        }

        public async Task<Subscription> UpdateSubscription(Subscription subscription)
        {
            using (var dc = DataContext())
            {
                dc.Subscriptions.Attach(subscription);
                dc.SetModified(subscription);
                await dc.SaveChangesAsync();
                return subscription;

            }
        }

        public async Task AddUserSubscription(ApplicationUser user, Subscription subscription,
            ApplicationUser modifiedByUser, string userHostAddress)
        {
            using (var dc = DataContext())
            {
                user = await _userManager.FindByIdAsync(user.Id);

                var originalSubscriptionUtc = user.SubscriptionUtc;
                var modifiedSubscriptionUtc = user.SubscriptionUtc.GetValueOrDefault(DateTime.Today).AddDays(subscription.Duration);

                user.SubscriptionUtc = modifiedSubscriptionUtc;

                dc.SubscriptionLogs.Add(new SubscriptionLog
                {
                    UserId = user.Id,
                    ModifiedByUserId = modifiedByUser.Id,
                    OriginalSubscriptionUtc = originalSubscriptionUtc,
                    ModifiedSubscriptionUtc = modifiedSubscriptionUtc,
                    UserHostAddress = userHostAddress
                });

                await dc.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets an indicator as to whether or not a subscription is valid for purchase.
        /// </summary>
        public static SubscriptionValidationResult ValidateSubscription(Subscription subscription)
        {
            var result = new SubscriptionValidationResult
            {
                IsValid = true
            };
            
            if (subscription == null)
            {
                result.IsValid = false;
                result.Reason = SubscriptionValidationResult.ReasonType.Null;
                
                return result;
            }

            if (subscription.Status != SubscriptionStatus.Active)
            {
                result.IsValid = false;
                result.Reason = SubscriptionValidationResult.ReasonType.Disabled;
                
                return result;
            }

            if (subscription.ExpirationUtc.HasValue && subscription.ExpirationUtc.Value < DateTime.UtcNow)
            {
                result.IsValid = false;
                result.Reason = SubscriptionValidationResult.ReasonType.Expired;
                
                return result;
            }

            if (subscription.Quantity.HasValue && subscription.Quantity.Value <= 0)
            {
                result.IsValid = false;
                result.Reason = SubscriptionValidationResult.ReasonType.NoQuantity;

                return result;
            }

            result.IsValid = true;

            return result;
        }

        public async Task BuySubscriptionWithTokens(Subscription subscription, ApplicationUser user, string userHostAddress = null)
        {
            var result = ValidateSubscription(subscription);
            if (!result.IsValid)
            {
                throw new Exception("Subscription is not valid for purchase.");
            }

            if (!subscription.PriceTokensIsValid)
            {
                throw new Exception("Subscription is not valid for purchase with tokens.");
            }

            if (subscription.PriceTokens > user.Tokens)
            {
                throw new Exception("Subscription cannot be purchased, user does not have enough available tokens.");
            }

            var order = await _orderService.CreateOrderForSubscription(subscription, user, PaymentMethod.Tokens, userHostAddress);

            await _tokenService.AddUserTokens(user, -(subscription.PriceTokens), user, userHostAddress);
            await AddUserSubscription(user, subscription, user, userHostAddress);

            // TODO: send email notification
        }
    }

    public class SubscriptionValidationResult
    {
        public bool IsValid { get; set; }
        public ReasonType Reason { get; set; }

        public enum ReasonType
        {
            Null,
            Disabled,
            Expired,
            NoQuantity
        }
    }
}
