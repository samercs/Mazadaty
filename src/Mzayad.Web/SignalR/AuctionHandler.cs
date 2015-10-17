using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Mzayad.Data;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Activity;
using Mzayad.Services.Identity;
using Mzayad.Web.Core.Caching;
using Mzayad.Web.Core.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mzayad.Web.Models;

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
        
        public AuctionHandler Setup(IDataContextFactory dataContextFactory, ICacheService cacheService)
        {
            _cacheService = _cacheService ?? cacheService;
            _auctionService = _auctionService ?? new AuctionService(dataContextFactory);
            _userService = _userService ?? new UserService(dataContextFactory);
            _bidService = _bidService ?? new BidService(dataContextFactory);
            _activityQueueService = _activityQueueService ?? 
                new ActivityQueueService(ConfigurationManager.ConnectionStrings["QueueConnection"].ConnectionString);

            return this;
        }

        public async Task<string> InitAuctions(int[] auctionIds)
        {   
            Trace.TraceInformation("InitAuctions(): {0}", JsonConvert.SerializeObject(auctionIds));
               
            var auctions = await _auctionService.GetLiveAuctions(auctionIds);
            var auctionModels = auctions.Select(LiveAuctionModel.Create).ToList();

            AddAuctionsToCache(auctionModels);

            var cachedAuctions = GetAuctionsFromCache();
            var auctionJson = Serialize(cachedAuctions);

            return auctionJson;
        }

        private IReadOnlyCollection<LiveAuctionModel> GetAuctionsFromCache()
        {
            var auctions = _cacheService.GetList<LiveAuctionModel>(CacheKeys.LiveAuctions);

            //Trace.TraceInformation("GetAuctionsFromCache(): {0}", JsonConvert.SerializeObject(auctions));

            return auctions;
        }

        private void AddAuctionsToCache(IEnumerable<LiveAuctionModel> auctions)
        {
            //Trace.TraceInformation("AddAuctionsToCache(): {0}", JsonConvert.SerializeObject(auctions));

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

                var cacheAuctions = _cacheService.GetList<LiveAuctionModel>(CacheKeys.LiveAuctions);
                if (cacheAuctions == null)
                {
                    //Trace.TraceInformation("UpdateAuctions(): no auctions in cache");

                    _updatingAuctions = false;
                    return;
                }

                foreach (var auction in cacheAuctions)
                {
                    //Trace.TraceInformation("UpdateAuctions(): processing {0}", JsonConvert.SerializeObject(auction));

                    auction.SecondsLeft = Math.Max(auction.SecondsLeft - 1, 0);
                    if (auction.SecondsLeft <= 0)
                    {
                        CloseAuction(auction);
                    }
                }

                _cacheService.SetList(CacheKeys.LiveAuctions, cacheAuctions);

                Clients.All.updateAuctions(Serialize(cacheAuctions));

                _updatingAuctions = false;
            }
        }

        private void CloseAuction(LiveAuctionModel auction)
        {
            var auctions = _cacheService.GetList<LiveAuctionModel>(CacheKeys.LiveAuctions);
            auctions = auctions.Where(i => i.AuctionId != auction.AuctionId).ToList();
            _cacheService.SetList(CacheKeys.LiveAuctions, auctions);

            var order = _auctionService.CloseAuction(auction.AuctionId).Result;

            Clients.All.closeAuction(auction.AuctionId, order.UserId, order.OrderId);
        }

        public async Task SubmitBid(int auctionId, string userId, string hostAddress)
        {
            //Trace.TraceInformation("SubmitBid(): UserId = {0}", userId);

            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var auctions = _cacheService.GetList<LiveAuctionModel>(CacheKeys.LiveAuctions);

            var auction = auctions.SingleOrDefault(i => i.AuctionId == auctionId);
            if (auction == null)
            {
                return;
            }

            var user = await _userService.GetUserById(userId);
            var bid = auction.AddBid(user);

            _cacheService.SetList(CacheKeys.LiveAuctions, auctions);
            
            await _bidService.AddBid(auctionId, userId, bid.BidAmount, auction.SecondsLeft, hostAddress);
            
            await _activityQueueService.QueueActivity(ActivityType.SubmitBid, userId , 5);// 5 points for submitting bid, it should be configured
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