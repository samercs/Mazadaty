using System;

namespace Mzayad.Core.Exceptions
{
    public class InsufficientTokensException : Exception
    {
        public InsufficientTokensException(string message) : base(message)
        {
        }
    }
}