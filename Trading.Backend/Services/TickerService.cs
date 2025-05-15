using System;
using System.ComponentModel;
using Trading.Common.Models;

namespace Trading.Backend.Services
{
    public class TickerService
    {
        static int NUM = 10;
        private Random _random;
        private List<Ticker> _tickers;
        public TickerService()
        {
            _random = new Random();
            _tickers = new List<Ticker>{
                new Ticker { Symbol = "AAPL", Price = 230.45f },
                new Ticker { Symbol = "MSFT", Price = 420.10f },
                new Ticker { Symbol = "GOOGL", Price = 175.20f },
                new Ticker { Symbol = "AMZN", Price = 190.50f },
                new Ticker { Symbol = "TSLA", Price = 340.75f },
                new Ticker { Symbol = "NVDA", Price = 135.30f },
                new Ticker { Symbol = "META", Price = 510.25f },
                new Ticker { Symbol = "JPM", Price = 205.15f },
                new Ticker { Symbol = "V", Price = 280.90f },
                new Ticker { Symbol = "WMT", Price = 75.60f }
            };
        }

        public List<Ticker> GetTickers() => _tickers;

        public List<Order> GetOrders(string tickerName)
        {
            List<Order> orders = new List<Order>();
            var ticker = _tickers.FirstOrDefault(t => t.Symbol == tickerName);
            for (int i = -NUM; i < NUM; i++)
            {
                var label = "Current";
                if (i > 0)
                {
                    label = "Bid " + Math.Abs(i);
                }
                else if(i < 0)
                {
                    label = "Ask " + Math.Abs(i);
                }
                orders.Add(new Order { Name = label, Price = ticker.Price + i, Quantity = _random.Next(1,3000) });
            }

            return orders;
        }
        

        public Ticker SelectTicker(string symbol, string connectionId)
        {
            var ticker = _tickers.FirstOrDefault(t => t.Symbol == symbol);

            return ticker;
        }

        public void UpdatePrices()
        {
            foreach (var ticker in _tickers)
            {
                // Simulate price change: ±0.5% to ±2% of current price
                double changePercent = (double)(_random.NextDouble() * 0.03 - 0.015); // -1.5% to +1.5%
                ticker.Price = (float)Math.Round((double)ticker.Price * (1 + changePercent), 2);
            }
        }
        
    }
}
