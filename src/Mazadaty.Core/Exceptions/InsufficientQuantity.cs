using System;

namespace Mazadaty.Core.Exceptions
{
    public class InsufficientQuantity : Exception
    {
        public InsufficientQuantity(string message) : base(message)
        {
        }
    }
}
