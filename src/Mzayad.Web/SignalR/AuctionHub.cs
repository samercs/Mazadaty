using System.Diagnostics;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace Mzayad.Web.SignalR
{
    public class AuctionHub : Hub
    {
        private readonly AuctionHandler _auctionHandler;

        public AuctionHub() : this(AuctionHandler.Instance) { }

        public AuctionHub(AuctionHandler auctionHandler)
        {
            _auctionHandler = auctionHandler;
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

        //    return base.OnConnected();
        //}

        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    Trace.TraceInformation("Disconnecting: " + Context.ConnectionId);

        //    UserHandler.ConnectedIds.Remove(Context.ConnectionId);

        //    return base.OnDisconnected(false);
        //}
    } 
}