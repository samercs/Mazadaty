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
using System.Threading.Tasks;
using Mzayad.Services.Identity;
using Mzayad.Web.Core.Trophies;
using OrangeJetpack.Services.Client.Messaging;

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
            
            var liveAuctions = new List<Auction>();
            var publicAuctions = await _auctionService.GetPublicAuctions(auctionIds);
            foreach (var auction in publicAuctions)
            {
                var cacheKey = string.Format(CacheKeys.LiveAuctionItem, auction.AuctionId);
                Trace.TraceInformation("processing cache key: " + cacheKey);

                if (!_cacheService.Exists(cacheKey))
                {
                    Trace.TraceInformation("cache key NOT exists");

                    _cacheService.Set(cacheKey, new Auction(auction));

                    _cacheService.AddToSet("LiveAuctionKeys", cacheKey);
                }
                else
                {
                    Trace.TraceInformation("cache key exists");
                }
            }

            var cacheKeys = _cacheService.GetSetMembers("LiveAuctionKeys");
            foreach (var cacheKey in cacheKeys)
            {
                Trace.TraceInformation("getting auction for cache key: " + cacheKey);
                
                var auction = _cacheService.Get<Auction>(cacheKey);
                
                Trace.TraceInformation(Serialize(auction));
                
                liveAuctions.Add(auction);
            }

            var serialize = Serialize(liveAuctions);

            Trace.TraceInformation(serialize);

            return serialize;
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

                var liveAuctions = new List<Auction>();

                var cacheKeys = _cacheService.GetSetMembers("LiveAuctionKeys").ToList();
                foreach (var cacheKey in cacheKeys)
                {
                    var auction = _cacheService.Get<Auction>(cacheKey);
                    
                    auction.SecondsLeft = Math.Max(auction.SecondsLeft - 1, 0);
                    if (auction.SecondsLeft <= 0)
                    {
                        CloseAuction(cacheKey, auction);
                    }

                    _cacheService.Set(cacheKey, auction);

                    liveAuctions.Add(auction);
                }

                Clients.All.updateAuctions(Serialize(liveAuctions));

                _updatingAuctions = false;
            }
        }

        private void CloseAuction(string cacheKey, Auction auction)
        {
            _cacheService.RemoveFromSet("LiveAuctionKeys", cacheKey);
            var task = _auctionService.CloseAuction(auction.AuctionId,
                () => _cacheService.Delete(CacheKeys.CurrentAuctions));

            var order = task.Result;

            Clients.All.closeAuction(auction.AuctionId, order.UserId, order.OrderId);
        }

        public async Task SubmitBid(int auctionId, string userId, string username,string hostAddress)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var cacheKey = string.Format(CacheKeys.LiveAuctionItem, auctionId);
            var auction = _cacheService.Get<Auction>(cacheKey);
            if (auction == null)
            {
                return;
            }

            var secondsLeft = auction.SecondsLeft;

            auction.AddBid(username);
            await _bidService.AddBid(auctionId, userId, auction.LastBidAmount.GetValueOrDefault(), secondsLeft, hostAddress);
            
            // Earn trophy
            var trophyEngine = new TrophiesEngine(_trophyService, _userService, _emailTemplateService, _messageService);
            trophyEngine.EarnTrophy(userId);

            _cacheService.Set(cacheKey, auction);
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