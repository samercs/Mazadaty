
namespace Mzayad.Web.Core.Configuration
{
    public class CacheKeys
    {
        public static string CurrentAuctions = "CurrentAuctions";
        public static string LiveAuctions = "LiveAuctions";
        public static string LiveAuctionItem = "LiveAuction:{0}";
        public static string UserSubscriptionUtc = "User:SubscriptionUtc:{0}";
        public const string SitePage = "page:{0}:{1}";
        public static string ShoppingCart = "shopping-cart:{0}"; // UserId or AnonymousId
    }
}
