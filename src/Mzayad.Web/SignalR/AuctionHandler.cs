using System.Threading.Tasks;
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

        public async Task<string> InitAuctions(int[] auctionIds)
        {
            var liveAuctions = GetCachedAuctions();

            await TryAddMissingAuctions(auctionIds, liveAuctions);

            return Serialize(liveAuctions);
        }

        private async Task TryAddMissingAuctions(IEnumerable<int> auctionIds, List<Auction> liveAuctions)
        {
            var missingIds = auctionIds.Except(liveAuctions.Select(i => i.AuctionId)).ToList();
            if (missingIds.Any())
            {
                var missingAuctions = await _auctionService.GetAuctions(missingIds);
                liveAuctions.AddRange(missingAuctions.Select(auction => new Auction(auction)));

                SetLiveAuctions(liveAuctions);
            }
        }

        private List<Auction> GetCachedAuctions()
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

            var liveAuctions = GetCachedAuctions();

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

                var liveAuctions = GetCachedAuctions();

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