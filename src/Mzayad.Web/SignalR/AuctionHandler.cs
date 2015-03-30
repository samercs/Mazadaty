using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Mzayad.Web.SignalR
{
    public class AuctionHandler
    {
        private readonly static Lazy<AuctionHandler> _instance =
            new Lazy<AuctionHandler>(() => new AuctionHandler(GlobalHost.ConnectionManager.GetHubContext<AuctionHub>().Clients));

        private readonly object _updateLock = new object();
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(2);
        private volatile bool _updatingAuctions;
        private readonly HashSet<Auction> _liveAuctions = new HashSet<Auction>();

        private IHubConnectionContext<dynamic> Clients { get; set; }

        private AuctionHandler(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            //Trace.TraceInformation("xxx");

            new Timer(UpdateAuctions, null, _updateInterval, _updateInterval);
        }

        public static AuctionHandler Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public string InitAuctions(int[] auctionIds)
        {
            foreach (var auctionId in auctionIds)
            {
                if (_liveAuctions.Any(i => i.AuctionId == auctionId)) // auction already exists in list
                {
                    continue;
                }

                // TODO get real auction

                var auction = new Auction
                {
                    AuctionId = auctionId,
                    StartUtc = DateTime.UtcNow.AddMinutes(-1),
                    Duration = 15
                };

                auction.SecondsLeft = (int)Math.Floor(auction.StartUtc.AddMinutes(auction.Duration).Subtract(DateTime.UtcNow).TotalSeconds);

                _liveAuctions.Add(auction);
            }

            return JsonConvert.SerializeObject(_liveAuctions, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public void SubmitBid(int auctionId, string userId = "anonymous")
        {
            Trace.TraceInformation("SubmitBid");
            Trace.TraceInformation(userId);

            var auction = _liveAuctions.SingleOrDefault(i => i.AuctionId == auctionId);
            if (auction == null)
            {
                return;
            }

            auction.LastBidAmount = (auction.LastBidAmount ?? 0) + 1;
            auction.LastBidderName = userId;
        }

        private void UpdateAuctions(object state)
        {
            lock (_updateLock)
            {
                if (_updatingAuctions)
                {
                    return;
                }

                _updatingAuctions = true;

                foreach (var auction in _liveAuctions)
                {
                    auction.SecondsLeft = (int)Math.Floor(auction.StartUtc.AddMinutes(auction.Duration).Subtract(DateTime.UtcNow).TotalSeconds);
                }

                BroadcastAuctionData();

                _updatingAuctions = false;
            }
        }

        private void BroadcastAuctionData()
        {
            //Trace.TraceInformation(dateTime.ToString(CultureInfo.InvariantCulture));
            //Trace.WriteLine("asdf");

            Clients.All.updateAuctions(JsonConvert.SerializeObject(_liveAuctions, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
        }
    }
}