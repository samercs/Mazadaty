using System;

namespace Mzayad.Core.Exceptions
{
    public class SubscriptionInvalidForPurchaseException : Exception
    {
        public SubscriptionInvalidForPurchaseException(string message) : base(message)
        {            
        }
    }
}