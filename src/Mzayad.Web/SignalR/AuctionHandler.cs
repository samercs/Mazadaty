using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Mzayad.Data;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Activity;
using Mzayad.Services.Identity;
using Mzayad.Web.Core.Caching;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mzayad.Models;

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
        private IActivityQueueService _activityQueueService;
        private AuctionService _auctionService;
        private UserService _userService;
        private BidService _bidService;
        private AutoBidService _autoBidService;

        public AuctionHandler Setup(IDataContextFactory dataContextFactory, ICacheService cacheService)
        {
            _cacheService = _cacheService ?? cacheService;
            _auctionService = _auctionService ?? new AuctionService(dataContextFactory);
            _userService = _userService ?? new UserService(dataContextFactory);
            _bidService = _bidService ?? new BidService(dataContextFactory);
            _autoBidService = _autoBidService ?? new AutoBidService(dataContextFactory);
            _activityQueueService = _activityQueueService ?? 
                new ActivityQueueService(ConfigurationManager.ConnectionStrings["QueueConnection"].ConnectionString);

            return this;
        }

        public async Task<string> InitAuctions(int[] auctionIds)
        {
            var cachedAuctions = GetAuctionsFromCache() ?? new List<LiveAuctionModel>();
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

        private IReadOnlyCollection<LiveAuctionModel> GetAuctionsFromCache()
        {
            return _cacheService.GetList<LiveAuctionModel>(CacheKeys.LiveAuctions);
        }

        private void SaveAuctionsToCache(IEnumerable<LiveAuctionModel> auctions)
        {
            _cacheService.SetList(CacheKeys.LiveAuctions, auctions);
        }

        private void  UpdateAuctions(object state)
        {
            lock (_updateLock)
            {
                if (_updatingAuctions)
                {
                    return;
                }

                _updatingAuctions = true;

                var cacheAuctions = GetAuctionsFromCache();
                if (cacheAuctions == null)
                {
                    _updatingAuctions = false;
                    return;
                }

                foreach (var auction in cacheAuctions)
                {
                    auction.SecondsLeft = Math.Max(auction.SecondsLeft - 1, 0);
                    if (auction.SecondsLeft <= 0)
                    {
                        CloseAuction(auction, cacheAuctions);
                        continue;
                    }

                    var user = _autoBidService.TryGetAutoBid(auction.AuctionId, auction.SecondsLeft);
                    if (user != null)
                    {
                        SubmitAutoBid(auction, user);
                    }
                }

                SaveAuctionsToCache(cacheAuctions);

                Clients.All.updateAuctions(Serialize(cacheAuctions));

                _updatingAuctions = false;
            }
        }

        private void CloseAuction(LiveAuctionModel auction, IEnumerable<LiveAuctionModel> auctions)
        {
            auctions = auctions.Where(i => i.AuctionId != auction.AuctionId).ToList();
            SaveAuctionsToCache(auctions);

            var order = _auctionService.CloseAuction(auction.AuctionId).Result;

            Clients.All.closeAuction(auction.AuctionId, order.UserId, order.OrderId);
        }

        public async Task SubmitBid(int auctionId, string userId, string hostAddress)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var auctions = GetAuctionsFromCache();

            var auction = auctions.SingleOrDefault(i => i.AuctionId == auctionId);
            if (auction == null)
            {
                return;
            }

            var user = await _userService.GetUserById(userId);
            var bid = auction.AddBid(user);

            await _bidService.AddBidAsync(auctionId, userId, bid.BidAmount, auction.SecondsLeft, hostAddress);
            await _activityQueueService.QueueActivityAsync(ActivityType.SubmitBid, userId);

            SaveAuctionsToCache(auctions);
        }

        private void SubmitAutoBid(LiveAuctionModel auction, ApplicationUser user)
        {
            var bid = auction.AddBid(user);

            _bidService.AddBid(auction.AuctionId, user.Id, bid.BidAmount, auction.SecondsLeft, "0.0.0.0");
            _activityQueueService.QueueActivity(ActivityType.AutoBid, user.Id);
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