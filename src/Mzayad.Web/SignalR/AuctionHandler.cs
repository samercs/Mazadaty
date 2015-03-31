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

        private ICacheService _cacheService;
        private AuctionService _auctionService;
        private BidService _bidService;
        
        public AuctionHandler Setup(IDataContextFactory dataContextFactory, ICacheService cacheService)
        {
            if (_cacheService == null)
            {
                _cacheService = cacheService;
            }

            if (_auctionService == null)
            {
                _auctionService = new AuctionService(dataContextFactory);
            }

            if (_bidService == null)
            {
                _bidService = new BidService(dataContextFactory);
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
                var missingAuctions = await _auctionService.GetPublicAuctions(missingIds);
                liveAuctions.AddRange(missingAuctions.Select(auction => new Auction(auction)));

                SetCacheAuctions(liveAuctions);
            }
        }

        private List<Auction> GetCachedAuctions()
        {
            return _cacheService.TryGet(CacheKeys.LiveAuctions, Enumerable.Empty<Auction>, TimeSpan.FromDays(1)).ToList();
        }

        private void SetCacheAuctions(IEnumerable<Auction> auctions)
        {
            _cacheService.Set(CacheKeys.LiveAuctions, auctions);
        }

        public async Task SubmitBid(int auctionId, string userId, string username)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }
            
            var auctions = GetCachedAuctions();
            var auction = auctions.SingleOrDefault(i => i.AuctionId == auctionId);
            if (auction == null)
            {
                return;
            }

            var secondsLeft = auction.SecondsLeft;

            auction.AddBid(username);

            await _bidService.AddBid(auctionId, userId, auction.LastBidAmount.GetValueOrDefault(), secondsLeft);

            SetCacheAuctions(auctions);
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
                    auction.SecondsLeft = Math.Max(auction.SecondsLeft - 1, 0);
                    if (auction.SecondsLeft == 0)
                    {
                        Trace.TraceInformation("Auction {0} should be closed", auction.AuctionId);
                    }
                }

                SetCacheAuctions(liveAuctions);

                UpdateClients(liveAuctions);

                _updatingAuctions = false;
            }
        }

        private void UpdateClients(IEnumerable<Auction> auctions)
        {
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