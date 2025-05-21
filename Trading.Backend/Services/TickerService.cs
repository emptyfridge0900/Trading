using Microsoft.EntityFrameworkCore;
using Trading.Backend.Interfaces;
using Trading.Backend.Models;
using Trading.Backend.Persistance;
using Trading.Common.Models;

namespace Trading.Backend.Services
{
    public class TickerService:ITickerService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<TickerService> _logger;
        private readonly List<Ticker> _tickerList;
        Store _store;
        public TickerService(IServiceScopeFactory factory, ILogger<TickerService> logger, Store store)
        {
            scopeFactory = factory;
            _logger = logger;
            _store = store;
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
            _tickerList = db.Tickers.ToList();
        }

        public async Task<PaginatedResult<Ticker>> GetTickers(int pageNum, int pageSize)
        {
            var Tickers = _tickerList;
            var totalCount = Tickers.Count();
            var tickers = Tickers
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize);

            var result = new PaginatedResult<Ticker>
            {
                Results = tickers.ToList(),
                PageNum = pageNum,
                PageSize = pageSize,

                IsPrevAvailable = pageNum > 1,
                IsNextAvailable = totalCount - ((pageNum - 1) * pageSize + pageSize) > 0,
            };

            return result;
        }

        public async Task<List<Order>> GetOrders(string tickerName, int numOfRow)
        {
            List<Order> orders = new List<Order>();
            var ticker = _tickerList.FirstOrDefault(t => t.Symbol.Equals(tickerName, StringComparison.OrdinalIgnoreCase));

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
        

    }
}
