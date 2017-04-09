using System;

namespace Mzayad.Core.Exceptions
{
    public class InsufficientQuantity : Exception
    {
        public InsufficientQuantity(string message) : base(message)
        {
        }
    }
}