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

        //public void SubmitBid(int auctionId)
        //{
        //    _auctionHandler.SubmitBid(auctionId);
        //}

        public override Task OnConnected()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);

            //Trace.TraceInformation(UserHandler.ConnectedIds.Count.ToString());

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);

            //Trace.TraceInformation(UserHandler.ConnectedIds.Count.ToString());

            return base.OnDisconnected(false);
        }
    } 
}