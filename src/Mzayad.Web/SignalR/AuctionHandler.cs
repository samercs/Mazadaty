using Kendo.Mvc.Infrastructure;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Mzayad.Data;
using Mzayad.Services;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
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
        private readonly static Lazy<AuctionHandler> _instance = new Lazy<AuctionHandler>(() => new AuctionHandler(GlobalHost.ConnectionManager.GetHubContext<AuctionHub>().Clients));
        private readonly object _updateLock = new object();
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);
        private volatile bool _updatingAuctions;
        private Timer _timer;

        private IHubConnectionContext<dynamic> Clients { get; set; }

        private AuctionHandler(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _timer = new Timer(UpdateAuctions, null, _updateInterval, _updateInterval);
        }

        public static AuctionHandler Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private AuctionService _auctionService;
        private ICacheService _cacheService;

        public AuctionHandler Setup(IDataContextFactory dataContextFactory, ICacheService cacheService)
        {
            if (_auctionService == null)
            {
                _auctionService = new AuctionService(dataContextFactory);
            }
            
            if (_cacheService == null)
            {
                _cacheService = cacheService;
            }

            return this;
        }

        public string InitAuctions(int[] auctionIds)
        {
            var liveAuctions = GetLiveAuctions();

            foreach (var auctionId in auctionIds)
            {
                if (liveAuctions.Any(i => i.AuctionId == auctionId)) // auction already exists in list
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

                auction.UpdateSecondsLeft();

                liveAuctions.Add(auction);
            }

            SetLiveAuctions(liveAuctions);

            return Serialize(liveAuctions);
        }

        private List<Auction> GetLiveAuctions()
        {
            return _cacheService.TryGet(CacheKeys.LiveAuctions, Enumerable.Empty<Auction>, TimeSpan.FromDays(1)).ToList();
        }

        private void SetLiveAuctions(IEnumerable<Auction> auctions)
        {
            _cacheService.Set(CacheKeys.LiveAuctions, auctions);
        }

        public void SubmitBid(int auctionId, string userId = "anonymous")
        {
            Trace.TraceInformation("SubmitBid");
            Trace.TraceInformation(userId);

            var liveAuctions = GetLiveAuctions();

            var auction = liveAuctions.SingleOrDefault(i => i.AuctionId == auctionId);
            if (auction == null)
            {
                return;
            }

            // TODO: log bid

            auction.LastBidAmount = (auction.LastBidAmount ?? 0) + 1;
            auction.LastBidderName = userId;

            SetLiveAuctions(liveAuctions);
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

                var liveAuctions = GetLiveAuctions();

                foreach (var auction in liveAuctions)
                {
                    auction.UpdateSecondsLeft();
                }

                SetLiveAuctions(liveAuctions);

                UpdateClients(liveAuctions);

                _updatingAuctions = false;
            }
        }

        private void UpdateClients(IEnumerable<Auction> auctions)
        {
            Trace.TraceInformation("UpdateClients " + DateTime.Now.Ticks);
            
            Clients.All.updateAuctions(Serialize(auctions));
        }

        private static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}