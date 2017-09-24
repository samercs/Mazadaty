using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Mazadaty.Core.Extensions;
using Mazadaty.Data;
using Mazadaty.Models;
using Mazadaty.Models.Enums;
using Mazadaty.Services;
using Mazadaty.Services.Activity;
using Mazadaty.Services.Identity;
using Mazadaty.Web.Core.Configuration;
using Mazadaty.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrangeJetpack.Base.Web.Caching;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mazadaty.Services.Queues;

namespace Mazadaty.Web.SignalR
{
    public class AuctionHandler
    {
        private static readonly Lazy<AuctionHandler> LazyInstance = new Lazy<AuctionHandler>(() => new AuctionHandler(GlobalHost.ConnectionManager.GetHubContext<AuctionHub>().Clients));
        private readonly object _updateLock = new object();
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(1);
        private volatile bool _updatingAuctions;
        private Timer _timer;

        private IHubConnectionContext<dynamic> Clients { get; }

        private AuctionHandler(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _timer = new Timer(UpdateAuctions, null, _updateInterval, _updateInterval);
        }

        public static AuctionHandler Instance => LazyInstance.Value;

        private ICacheService _cacheService;
        private IQueueService _queueService;
        private AuctionService _auctionService;
        private UserService _userService;
        private BidService _bidService;
        private AutoBidService _autoBidService;
        
        public AuctionHandler Setup(IDataContextFactory dataContextFactory, ICacheService cacheService)
        {
            Trace.TraceInformation("AuctionHandler.Setup()");

            _cacheService = _cacheService ?? cacheService;
            _queueService = _queueService ?? new QueueService(ConfigurationManager.ConnectionStrings["QueueConnection"].ConnectionString);
            _auctionService = _auctionService ?? new AuctionService(dataContextFactory, _queueService);
            _userService = _userService ?? new UserService(dataContextFactory);
            _bidService = _bidService ?? new BidService(dataContextFactory, _queueService);
            _autoBidService = _autoBidService ?? new AutoBidService(dataContextFactory);
            
            return this;
        }

        public async Task<string> InitAuctions(int[] auctionIds)
        {
            Trace.TraceInformation("AuctionHandler.InitAuctions(): " + string.Join(", ", auctionIds));

            try
            {
                var cachedAuctions = GetAuctionsFromCache();
                var missingIds = auctionIds.Except(cachedAuctions.Select(i => i.AuctionId)).ToList();
                if (!missingIds.Any())
                {
                    return Serialize(cachedAuctions);
                }

                var auctions = await _auctionService.GetLiveAuctions(missingIds);
                var auctionModels = auctions.Select(LiveAuctionModel.Create).ToList();

                auctionModels.AddRange(cachedAuctions);

                SaveAuctionsToCache(auctionModels);

                return Serialize(auctionModels);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);

                return Serialize(new List<LiveAuctionModel>());
            }
        }

        private List<LiveAuctionModel> GetAuctionsFromCache()
        {
            return _cacheService.TryGetList(CacheKeys.LiveAuctions, () => Enumerable.Empty<LiveAuctionModel>().ToList()).ToList();
        }

        private void SaveAuctionsToCache(IEnumerable<LiveAuctionModel> auctions)
        {
            if (auctions.IsNullOrEmpty())
            {
                _cacheService.Delete(CacheKeys.LiveAuctions);
                return;
            }

            _cacheService.SetList(CacheKeys.LiveAuctions, auctions);
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

                var auctions = GetAuctionsFromCache();
                if (auctions == null)
                {
                    _updatingAuctions = false;
                    return;
                }

                foreach (var auction in auctions.ToList())
                {
                    auction.UpdateSecondsLeft();

                    if (auction.SecondsLeft < 0)
                    {
                        CloseAuction(auction.AuctionId);

                        auctions.RemoveAll(i => i.AuctionId == auction.AuctionId);

                        continue;
                    }

                    var bid = _bidService.TrySubmitAutoBid(auction.AuctionId, auction.SecondsLeft);
                    if (bid != null)
                    {
                        auction.AddBid(bid);
                    }      
                }

                SaveAuctionsToCache(auctions);

                Clients.All.updateAuctions(Serialize(auctions));

                _updatingAuctions = false;
            }
        }

        private void CloseAuction(int auctionId)
        {
            var order = _auctionService.CloseAuction(auctionId).Result;

            Clients.All.closeAuction(auctionId, order?.UserId, order?.OrderId);

            if (order != null)
            {
                //_activityQueueService.QueueActivity(ActivityType.WinAuction, order.UserId);
            }
        }

        public void SubmitBid(int auctionId, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var auctions = GetAuctionsFromCache();

            var auction = auctions.SingleOrDefault(i => i.AuctionId == auctionId);

            SubmitUserBid(auction, userId);

            SaveAuctionsToCache(auctions);
        }

        private void SubmitUserBid(LiveAuctionModel auction, string userId)
        {
            if (auction == null)
            {
                return;
            }

            var bid = _bidService.SubmitUserBid(auction.AuctionId, auction.SecondsLeft, userId);
            if (bid != null)
            {
                auction.AddBid(bid);
            }
        }

        private static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}
