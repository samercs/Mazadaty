using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Mzayad.Web
{
    public class StockTicker
    {
        // Singleton instance
        private readonly static Lazy<StockTicker> _instance = 
            new Lazy<StockTicker>(() => new StockTicker(GlobalHost.ConnectionManager.GetHubContext<StockTickerHub>().Clients));

        private readonly object _updateStockPricesLock = new object();

        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(2);
        
        //private readonly Timer _timer;
        private volatile bool _updatingStockPrices;

        private StockTicker(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            //Trace.TraceInformation("xxx");

            new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);
        }

        public static StockTicker Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> Clients { get; set; }

        public DateTime GetAllStocks()
        {
            return DateTime.Now;
        }

        private void UpdateStockPrices(object state)
        {
            lock (_updateStockPricesLock)
            {
                if (!_updatingStockPrices)
                {
                    _updatingStockPrices = true;

                    BroadcastStockPrice(DateTime.Now);

                    _updatingStockPrices = false;
                }
            }
        }

        private void BroadcastStockPrice(DateTime dateTime)
        {
            //Trace.TraceInformation(dateTime.ToString(CultureInfo.InvariantCulture));
            //Trace.WriteLine("asdf");
            
            Clients.All.updateStockPrice(dateTime);
        }

        

    }

    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }





    public class StockTickerHub : Hub
    {
        private readonly StockTicker _stockTicker;

        public StockTickerHub() : this(StockTicker.Instance) { }

        public StockTickerHub(StockTicker stockTicker)
        {
            _stockTicker = stockTicker;
        }

        public DateTime GetAllStocks()
        {
            return _stockTicker.GetAllStocks();
        }

        public override Task OnConnected()
        {
            UserHandler.ConnectedIds.Add(Context.ConnectionId);

            //Trace.TraceInformation(UserHandler.ConnectedIds.Count.ToString());

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool x)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);

            //Trace.TraceInformation(UserHandler.ConnectedIds.Count.ToString());

            return base.OnDisconnected(false);
        }
    }
    
    
    
    
    
    
    
    
    
    public class Stock
    {
        private decimal _price;

        public string Symbol { get; set; }

        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                if (_price == value)
                {
                    return;
                }

                _price = value;

                if (DayOpen == 0)
                {
                    DayOpen = _price;
                }
            }
        }

        public decimal DayOpen { get; private set; }

        public decimal Change
        {
            get
            {
                return Price - DayOpen;
            }
        }

        public double PercentChange
        {
            get
            {
                return (double)Math.Round(Change / Price, 4);
            }
        }
    }
}
