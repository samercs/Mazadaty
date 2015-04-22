using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Mzayad.Data;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.SignalR
{
    public class AuctionHub : Hub
    {
        private readonly AuctionHandler _auctionHandler;

        public AuctionHub(IDataContextFactory dataContextFactory, ICacheService cacheService) :
            this(dataContextFactory, cacheService, AuctionHandler.Instance)
        {
        }

        public AuctionHub(IDataContextFactory dataContextFactory, ICacheService cacheService, AuctionHandler auctionHandler)
        {
            _auctionHandler = auctionHandler.Setup(dataContextFactory, cacheService);
        }

        public async Task ClearAuctionCache()
        {
            await _auctionHandler.ClearAuctionCache();
        }

        public async Task<string> InitAuctions(int[] auctionIds)
        {
            return await _auctionHandler.InitAuctions(auctionIds);
        }

        public async Task SubmitBid(int auctionId)
        {
            var identity = Context.User.Identity;
            var userId = identity.GetUserId();
            var username = identity.GetUserName();
            var hostAddress = GetIpAddress();
            
            
            await _auctionHandler.SubmitBid(auctionId, userId, username,hostAddress);
        }

        public string GetUsername()
        {
            return Context.User.Identity.GetUserName();
        }

        protected string GetIpAddress()
        {
            string ipAddress;
            object tempObject;

            Context.Request.Environment.TryGetValue("server.RemoteIpAddress", out tempObject);

            if (tempObject != null)
            {
                ipAddress = (string)tempObject;
            }
            else
            {
                ipAddress = "";
            }

            return ipAddress;
        }

        

        //public override Task OnConnected()
        //{
        //    Trace.TraceInformation("Connecting: " + Context.ConnectionId);

        //    UserHandler.ConnectedIds.Add(Context.ConnectionId);

        //    Clients.Caller.onConnected("Welcome on " + DateTime.UtcNow);

        //    return base.OnConnected();
        //}

        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    Trace.TraceInformation("Disconnecting: " + Context.ConnectionId);

        //    UserHandler.ConnectedIds.Remove(Context.ConnectionId);

        //    Clients.Caller.onDisconnected("Disconnected on " + DateTime.UtcNow);

        //    return base.OnDisconnected(false);
        //}
    } 
}