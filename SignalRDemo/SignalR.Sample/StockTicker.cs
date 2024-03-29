﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

using SignalR;

namespace SignalRDemo.SignalR.Sample
{
    public class StockTicker
    {
        private readonly static Lazy<StockTicker> _instance = new Lazy<StockTicker>(() => new StockTicker());
        private readonly static object MarketStateLock = new object();
        private readonly ConcurrentDictionary<string, Stock> _stocks = new ConcurrentDictionary<string, Stock>();
        private const double RangePercent = .008; //stock can go up or down by a percentage of this factor on each change
        private const int UpdateInterval = 250; //ms
        // This is used as an singleton instance so we'll never both disposing the timer
        private Timer _timer;
        private readonly object _updateStockPricesLock = new object();
        private bool _updatingStockPrices = false;
        private readonly Random _updateOrNotRandom = new Random();
        private MarketState _marketState = MarketState.Closed;

        private StockTicker()
        {
            LoadDefaultStocks();
        }

        public static StockTicker Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public MarketState MarketState
        {
            get { return _marketState; }
            private set { _marketState = value; }
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stocks.Values;
        }

        public void OpenMarket()
        {
            if (MarketState != MarketState.Open || MarketState != MarketState.Opening)
            {
                lock (MarketStateLock)
                {
                    if (MarketState != MarketState.Open || MarketState != MarketState.Opening)
                    {
                        MarketState = MarketState.Opening;
                        _timer = new Timer(UpdateStockPrices, null, UpdateInterval, UpdateInterval);
                        MarketState = MarketState.Open;
                        BroadcastMarketStateChange(MarketState.Open);
                    }
                }
            }
        }

        public void CloseMarket()
        {
            if (MarketState == MarketState.Open || MarketState == MarketState.Opening)
            {
                lock (MarketStateLock)
                {
                    if (MarketState == MarketState.Open || MarketState == MarketState.Opening)
                    {
                        MarketState = MarketState.Closing;
                        if (_timer != null)
                        {
                            _timer.Dispose();
                        }
                        MarketState = MarketState.Closed;
                        BroadcastMarketStateChange(MarketState.Closed);
                    }
                }
            }
        }

        public void Reset()
        {
            lock (MarketStateLock)
            {
                if (MarketState != MarketState.Closed)
                {
                    throw new InvalidOperationException("Market must be closed before it can be reset.");
                }
                _stocks.Clear();
                LoadDefaultStocks();
                BroadcastMarketStateChange(MarketState.Reset);
            }
        }

        private void LoadDefaultStocks()
        {
            new List<Stock>
            {
                new Stock { Symbol = "MSFT", Price = 30.31m },
                new Stock { Symbol = "APPL", Price = 578.18m },
                new Stock { Symbol = "GOOG", Price = 570.30m }
            }.ForEach(stock => _stocks.TryAdd(stock.Symbol, stock));
        }

        private void UpdateStockPrices(object state)
        {
            // This function must be re-entrant as it's running as a timer interval handler
            if (_updatingStockPrices)
            {
                return;
            }

            lock (_updateStockPricesLock)
            {
                if (!_updatingStockPrices)
                {
                    _updatingStockPrices = true;

                    foreach (var stock in _stocks.Values)
                    {
                        if (UpdateStockPrice(stock))
                        {
                            BroadcastStockPrice(stock);
                        }
                    }

                    _updatingStockPrices = false;
                }
            }
        }

        private bool UpdateStockPrice(Stock stock)
        {
            // Randomly choose whether to udpate this stock or not
            var r = _updateOrNotRandom.NextDouble();
            if (r > .1)
            {
                return false;
            }

            // Update the stock price by a random factor of the range percent
            var random = new Random((int)Math.Floor(stock.Price));
            var percentChange = random.NextDouble() * RangePercent;
            var pos = random.NextDouble() > .51;
            var change = Math.Round(stock.Price * (decimal)percentChange, 2);
            change = pos ? change : -change;

            stock.Price += change;
            return true;
        }

        private static void BroadcastMarketStateChange(MarketState marketState)
        {
            var clients = GetClients();
            switch (marketState)
            {
                case MarketState.Open:
                    clients.marketOpened();
                    break;
                case MarketState.Closed:
                    clients.marketClosed();
                    break;
                case MarketState.Reset:
                    clients.marketReset();
                    break;
                default:
                    break;
            }
        }

        private static void BroadcastStockPrice(Stock stock)
        {
            GetClients().updateStockPrice(stock);
        }

        private static dynamic GetClients()
        {
            return GlobalHost.ConnectionManager.GetHubContext<StockTickerHub>().Clients;
        }
    }

    public enum MarketState
    {
        Open,
        Opening,
        Closing,
        Closed,
        Reset
    }
}