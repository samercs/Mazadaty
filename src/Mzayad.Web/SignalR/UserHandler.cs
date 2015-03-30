using System.Collections.Generic;

namespace Mzayad.Web.SignalR
{
    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }
}