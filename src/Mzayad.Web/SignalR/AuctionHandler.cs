using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Mzayad.Core.Extensions;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Activity;
using Mzayad.Services.Identity;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Models;
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
using Mzayad.Services.Queues;

namespace Mzayad.Web.SignalR
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

                    if (!_autoBidService.ShouldAutoBid(auction.SecondsLeft))
                    {
                        continue;
                    }

                    var userId = _autoBidService.TryGetAutoBid(auction.AuctionId, auction.LastBidUserId, auction.LastBidAmount);
                    SubmitBid(auction, userId, BidType.Auto);          
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

        private void SubmitBid(LiveAuctionModel auction, string userId, BidType bidType, string hostAddress = "0.0.0.0")
        {
            if (auction == null || string.IsNullOrEmpty(userId))
            {
                return;
            }

            if (auction.LastBidUserId == userId)
            {
                return;
            }

            var bid = _bidService.AddBid(new Bid
            {
                AuctionId = auction.AuctionId,
                UserId = userId,
                Amount = auction.GetNewBidAmount(),
                SecondsLeft = auction.SecondsLeft,
                UserHostAddress = hostAddress,
                Type = bidType
            });

            if (bid == null)
            {
                return;
            }

            auction.AddBid(bid);
            //await _activityQueueService.QueueActivityAsync(ActivityType.SubmitBid, userId);
            //await _activityQueueService.QueueActivityAsync(ActivityType.EarnXp, userId, NormalBidXpPoints);
        }

        public void SubmitBid(int auctionId, string userId, BidType bidType, string hostAddress)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var auctions = GetAuctionsFromCache();

            var auction = auctions.SingleOrDefault(i => i.AuctionId == auctionId);

            SubmitBid(auction, userId, bidType, hostAddress);

            SaveAuctionsToCache(auctions);
        }

        //private void SubmitAutoBid(LiveAuctionModel auction, ApplicationUser user)
        //{
        //    var bid = new Bid
        //    {
        //        AuctionId = auction.AuctionId,
        //        UserId = user.Id,
        //        Amount = auction.GetNewBidAmount(),
        //        SecondsLeft = auction.SecondsLeft,
        //        UserHostAddress = "0.0.0.0",
        //        Type = BidType.Auto
        //    };

        //    //if (_bidService.AddBid(bid))
        //    //{
        //    //    auction.AddBid(user);
        //    //    //_activityQueueService.QueueActivity(ActivityType.AutoBid, user.Id);
        //    //    //_activityQueueService.QueueActivityAsync(ActivityType.EarnXp, user.Id, AutoBidXpPoints).ContinueWith(result => { });
        //    //}
        //}

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