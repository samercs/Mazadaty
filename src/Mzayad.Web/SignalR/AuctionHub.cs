using Microsoft.AspNet.SignalR;
using Mzayad.Data;
using Mzayad.Web.Core.Caching;
using System.Diagnostics;
using System.Threading.Tasks;

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


        public async Task<string> InitAuctions(int[] auctionIds)
        {
            return await _auctionHandler.InitAuctions(auctionIds);
        }

        public async Task SubmitBid(int auctionId, string userId)
        {
            //Trace.TraceInformation("SubmitBid(): AuctionId = {0}, UserId = {1}", auctionId, userId);

            //var identity = Context.User.Identity;
            //var userId = identity.GetUserId();
            var hostAddress = GetIpAddress();
                 
            await _auctionHandler.SubmitBid(auctionId, userId, hostAddress);
        }

        protected string GetIpAddress()
        {
            var ipAddress = "";
            object tempObject;

            if (Context.Request.Environment.TryGetValue("server.RemoteIpAddress", out tempObject))
            {
                ipAddress = tempObject.ToString();
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