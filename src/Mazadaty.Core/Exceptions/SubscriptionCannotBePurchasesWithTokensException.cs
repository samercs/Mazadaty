using System;

namespace Mazadaty.Core.Exceptions
{
    public class SubscriptionCannotBePurchasesWithTokensException : Exception
    {
        public SubscriptionCannotBePurchasesWithTokensException(string message) : base(message)
        {
        }
    }
}
