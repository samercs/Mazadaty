using System;

namespace Mzayad.Core.Exceptions
{
    public class UserNotHaveAvatarPrize : Exception
    {
        public UserNotHaveAvatarPrize(string message) : base(message)
        {

        }
    }
}