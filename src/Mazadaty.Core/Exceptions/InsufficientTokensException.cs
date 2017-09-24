using System;

namespace Mazadaty.Core.Exceptions
{
    public class InsufficientTokensException : Exception
    {
        public InsufficientTokensException(string message) : base(message)
        {
        }
    }
}
