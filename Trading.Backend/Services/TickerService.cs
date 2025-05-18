using Trading.Backend.Interfaces;
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
        public TickerService(IServiceScopeFactory factory, ILogger<TickerService> logger)
        {
            scopeFactory = factory;
            _random = new Random();
            _logger = logger;
        }

        public List<Ticker> GetTickers()
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();

            return db.Tickers.ToList();
        }

        public List<Order> GetOrders(string tickerName)
        {
            List<Order> orders = new List<Order>();
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
            var ticker = db.Tickers.FirstOrDefault(t => t.Symbol == tickerName);
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
