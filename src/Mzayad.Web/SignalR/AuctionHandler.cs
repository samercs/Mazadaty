using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Mzayad.Data;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Services.Activity;
using Mzayad.Services.Identity;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrangeJetpack.Services.Client.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

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
        private TrophyService _trophyService;
        private EmailTemplateService _emailTemplateService;
        private UserService _userService;
        private MessageService _messageService;
        private IActivityQueueService _activityQueueService;

        public AuctionHandler Setup(IDataContextFactory dataContextFactory, ICacheService cacheService)
        {
            _cacheService = _cacheService ?? cacheService;
            _auctionService = _auctionService ?? new AuctionService(dataContextFactory);
            _bidService = _bidService ?? new BidService(dataContextFactory);

            if (_trophyService == null)
            {
                _trophyService = new TrophyService(dataContextFactory);
            }

            if (_emailTemplateService == null)
            {
                _emailTemplateService = new EmailTemplateService(dataContextFactory);
            }

            if (_userService == null)
            {
                _userService = new UserService(dataContextFactory);
            }

            if (_messageService == null)
            {
                _messageService = new MessageService(new EmailSettings());
            }

            if (_activityQueueService == null)
            {
                _activityQueueService = new ActivityQueueService(ConfigurationManager.ConnectionStrings["QueueConnection"].ConnectionString);
            }

            return this;
        }

        public async Task ClearAuctionCache()
        {
            var cacheKeys = _cacheService.GetSetMembers("LiveAuctionKeys");
            foreach (var cacheKey in cacheKeys)
            {
                await _cacheService.Delete(cacheKey);
            }

            await _cacheService.Delete("LiveAuctionKeys");
        }

        public async Task<string> InitAuctions(int[] auctionIds)
        {      
            Trace.TraceInformation("InitAuctions(" + string.Join(",", auctionIds) + ")");
            
            var auctions = await _auctionService.GetLiveAuctions(auctionIds);
            
            Trace.TraceInformation("Auctions: " + auctions.Count());

            try
            {
                AddAuctionsToCache(auctions.Select(AuctionModel.Create));
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            var auctionJson = Serialize(GetAuctionsFromCache());

            Trace.TraceInformation(auctionJson);

            return auctionJson;
        }

        private IReadOnlyCollection<AuctionModel> GetAuctionsFromCache()
        {
            Trace.TraceInformation("GetAuctionsFromCache()");

            return _cacheService.GetList<AuctionModel>(CacheKeys.LiveAuctions);

            //var cacheKeys = _cacheService.GetSetMembers("LiveAuctionKeys");
            //foreach (var cacheKey in cacheKeys)
            //{
            //    Trace.TraceInformation("getting auction for cache key: " + cacheKey);

            //    var auction = _cacheService.Get<AuctionModel>(cacheKey);

            //    Trace.TraceInformation(Serialize(auction));

            //    yield return auction;
            //}
        }

        private void AddAuctionsToCache(IEnumerable<AuctionModel> auctions)
        {
            Trace.TraceInformation("AddAuctionsToCache(): " + JsonConvert.SerializeObject(auctions));

            _cacheService.SetList(CacheKeys.LiveAuctions, auctions);

            //foreach (var auction in publicAuctions)
            //{
            //    var cacheKey = string.Format(CacheKeys.LiveAuctionItem, auction.AuctionId);
            //    Trace.TraceInformation("processing cache key: " + cacheKey);

            //    if (!_cacheService.Exists(cacheKey))
            //    {
            //        Trace.TraceInformation("cache key NOT exists");

            //        _cacheService.Set(cacheKey, AuctionModel.Create(auction));
            //        _cacheService.AddToSet("LiveAuctionKeys", cacheKey);
            //    }
            //    else
            //    {
            //        Trace.TraceInformation("cache key exists");
            //    }
            //}
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

                var cacheAuctions = _cacheService.GetList<AuctionModel>(CacheKeys.LiveAuctions);
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
                        CloseAuction("", auction);
                    }

                    auction.Bids = new Queue<BidModel>(auction.Bids.OrderByDescending(i => i.BidAmount));
                }

                _cacheService.SetList(CacheKeys.LiveAuctions, cacheAuctions);

                //var cacheKeys = _cacheService.GetSetMembers("LiveAuctionKeys").ToList();
                //foreach (var cacheKey in cacheKeys)
                //{
                //    var auction = _cacheService.Get<AuctionModel>(cacheKey);

                //    auction.SecondsLeft = Math.Max(auction.SecondsLeft - 1, 0);
                //    if (auction.SecondsLeft <= 0)
                //    {
                //        CloseAuction(cacheKey, auction);
                //    }

                //    _cacheService.Set(cacheKey, auction);

                //    liveAuctions.Add(auction);
                //}

                //foreach (var auction in liveAuctions)
                //{
                //    auction.Bids = new Queue<BidModel>(auction.Bids.OrderByDescending(i => i.BidAmount));
                //}

                Clients.All.updateAuctions(Serialize(cacheAuctions));

                _updatingAuctions = false;
            }
        }

        private void CloseAuction(string cacheKey, AuctionModel auction)
        {
            var auctions = _cacheService.GetList<AuctionModel>(CacheKeys.LiveAuctions);
            auctions = auctions.Where(i => i.AuctionId != auction.AuctionId).ToList();
            _cacheService.SetList(CacheKeys.LiveAuctions, auctions);

            var order = _auctionService.CloseAuction(auction.AuctionId).Result;

            

            //_cacheService.RemoveFromSet("LiveAuctionKeys", cacheKey);
            //var task = _auctionService.CloseAuction(auction.AuctionId,
            //    () => _cacheService.Delete(CacheKeys.CurrentAuctions));

            //var order = task.Result;

            Clients.All.closeAuction(auction.AuctionId, order.UserId, order.OrderId);
        }

        public async Task SubmitBid(int auctionId, string userId, string hostAddress)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var auctions = _cacheService.GetList<AuctionModel>(CacheKeys.LiveAuctions);

            var auction = auctions.SingleOrDefault(i => i.AuctionId == auctionId);
            if (auction == null)
            {
                return;
            }

            var user = await _userService.GetUserById(userId);
            var bid = auction.AddBid(user);

            _cacheService.SetList(CacheKeys.LiveAuctions, auctions);
            
            await _bidService.AddBid(auctionId, userId, bid.BidAmount, auction.SecondsLeft, hostAddress);
            
            // Earn trophy
            //var trophyEngine = new TrophiesEngine(_trophyService, _userService, _emailTemplateService, _messageService);
            //trophyEngine.EarnTrophy(userId);

            await _activityQueueService.QueueActivity(ActivityType.SubmitBid, userId);



            //_cacheService.Set(cacheKey, auction);
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