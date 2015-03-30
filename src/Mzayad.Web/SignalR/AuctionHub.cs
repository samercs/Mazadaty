using Microsoft.AspNet.SignalR;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.SignalR
{
    public class AuctionHub : Hub
    {
        private readonly AuctionHandler _auctionHandler;

        public AuctionHub(ICacheService cacheService) : this(cacheService, AuctionHandler.Instance) { }

        public AuctionHub(ICacheService cacheService, AuctionHandler auctionHandler)
        {
            _auctionHandler = auctionHandler;
            _auctionHandler.CacheService = cacheService;
        }

        public string InitAuctions(int[] auctionIds)
        {
            return _auctionHandler.InitAuctions(auctionIds);
        }

        public void SubmitBid(int auctionId)
        {
            var userId = Context.User.Identity.Name;

            _auctionHandler.SubmitBid(auctionId, userId);
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