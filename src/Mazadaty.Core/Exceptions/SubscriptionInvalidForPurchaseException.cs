using System;

namespace Mazadaty.Core.Exceptions
{
    public class SubscriptionInvalidForPurchaseException : Exception
    {
        public SubscriptionInvalidForPurchaseException(string message) : base(message)
        {            
        }
    }
}
