namespace Mazadaty.Web.Areas.Api.ErrorHandling
{
    public enum ApiErrorType
    {
        ModelStateError,
        InsufficientTokensError,
        SubscriptionNotValidForPurchase,
        SubscriptionNotValidForPurchaseWithTokens
    }
}
