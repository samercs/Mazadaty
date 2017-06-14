using System;

namespace Mzayad.Core.Exceptions
{
    public class SubscriptionCannotBePurchasesWithTokensException : Exception
    {
        public SubscriptionCannotBePurchasesWithTokensException(string message) : base(message)
        {
        }
    }
}