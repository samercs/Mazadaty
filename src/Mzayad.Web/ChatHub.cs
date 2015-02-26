using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Threading;

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
            Clients.All.updateStockPrice(dateTime);
        }

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
