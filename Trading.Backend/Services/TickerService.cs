using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Trading.Backend.Interfaces;
using Trading.Backend.Models;
using Trading.Backend.Persistance;
using Trading.Common.Models;

namespace Trading.Backend.Services
{
    public class TickerService:ITickerService
    {
        static int NUM = 10;
        private Random _random;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<TickerService> _logger;
        Store _store;
        public TickerService(IServiceScopeFactory factory, ILogger<TickerService> logger, Store store)
        {
            scopeFactory = factory;
            _random = new Random();
            _logger = logger;
            _store = store;
        }

        public List<Ticker> GetTickers()
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();

            return db.Tickers.ToList();
        }

        public List<Order> GetOrders(string tickerName, int numOfRow)
        {
            List<Order> orders = new List<Order>();
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
            var ticker = db.Tickers.FirstOrDefault(t => t.Symbol == tickerName);

            if (_store.Stocks.ContainsKey(ticker.Symbol))
            {
                var stock = _store.Stocks[ticker.Symbol];
                foreach(var bid in stock.Bids.Values)
                {
                    if (bid.Total > 0)
                    {
                        orders.Add(new Order { Name = "Bid", Price = bid.Price, Quantity = bid.Total });
                        if (orders.Count == numOfRow)
                            break;
                    }
                }

                foreach(var ask in stock.Asks.Values)
                {
                    if(ask.Total>0)
                    {
                        orders.Add(new Order { Name = "Ask", Price = ask.Price, Quantity = ask.Total });
                        if (orders.Count == 2* numOfRow)
                            break;
                    }
                }
            }

            return orders.OrderByDescending(x => x.Price).ToList();
        }
        


        public void UpdatePrices()
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
            foreach (var ticker in db.Tickers)
            {
                // Simulate price change: ±0.5% to ±2% of current price
                double changePercent = (double)(_random.NextDouble() * 0.03 - 0.015); // -1.5% to +1.5%
                ticker.Price = (float)Math.Round((double)ticker.Price * (1 + changePercent), 2);
            }
        }
        
    }
}
