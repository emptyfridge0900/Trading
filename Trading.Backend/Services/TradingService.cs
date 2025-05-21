using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Trading.Backend.Hubs;
using Trading.Backend.Interfaces;
using Trading.Backend.Models;
using Trading.Backend.Persistance;
using Trading.Common.Models;

namespace Trading.Backend.Services
{
    
    
    public class TradingService:ITradingService
    {
        private readonly IHubContext<TradingHub, ITrade> _hubContext;
        Store _store;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TradingService> _logger;
        public TradingService(IServiceScopeFactory factory, ILogger<TradingService> logger, 
            IHubContext<TradingHub, ITrade> hubContext, Store store)
        {
            _scopeFactory = factory;
            _logger = logger;
            _store = store;
            _hubContext = hubContext;

        }
        public async Task<List<TradeRecord>> GetTradingRecords(string userId)
        {
            using var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TradingDbContext>();
            var records = await db.Records.Where(r => r.UserId == userId).ToListAsync();
            //there is 0 possibility that a user doesn't have any records. Because when a user is created, random records are given
            records.Reverse();
            return records;
            
        }

        public async Task AddRecord(TradeRecord record)
        {
            using var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TradingDbContext>();
            //assume that all the user inputs are valid
            await db.Records.AddAsync(record);
            
            await db.SaveChangesAsync();
        }
        public async Task Bid(string userId, string symbol, float price, int quantity)
        {
            if (_store.Stocks.ContainsKey(symbol))
            {
                var s = _store.Stocks[symbol];
                s.UpdateCurrentPrice();
                //before starting bid, check the bid price is in ask price
                //let's say current price is $430 and you want to bid $420
                //look for ask order that price between 430 and 420. If there is a ask order at price $428
                //then we process the $428 ask order first, sell it at better price!
                var inRangeAsks = s.Asks.Where(x => (s.CurrentPrice >= x.Key && x.Key >= price)).OrderByDescending(x=>x.Key).Select(x=>x.Value);
                foreach (var i in inRangeAsks)
                {
                    if (i.Total > 0)
                    {
                        var asks = i.Take(quantity, out var unredeemed);
                        foreach (var item in asks)
                        {
                            await _hubContext.Clients.User(item.Asker).ReceiveRecords(await GetTradingRecords(item.Asker));
                        }
                        s.UpdateCurrentPrice();
                        quantity = unredeemed;
                    }
                }



                //if someone has already bid 10 Teslas at $420,
                //add your bid to the queue and increase to the total bid quantity.
                if (s.Bids.ContainsKey(price))
                {
                    var collection = s.Bids[price];
                    if(quantity > 0)
                    {
                        collection.Add(new Bid(userId,price,quantity));
                    }
                    
                }
                else
                {
                    s.Bids.Add(price, new BidCollection(price));
                    var collection = s.Bids[price];
                    if (quantity > 0)
                    {
                        collection.Add(new Bid(userId,price,quantity));
                    }
                }

            }
        }
        public async Task Ask(string userId, string symbol, float price, int quantity)
        {
            if (_store.Stocks.ContainsKey(symbol))
            {
                var s = _store.Stocks[symbol];
                s.UpdateCurrentPrice();
                var inRangeBids = s.Bids.Where(x => (s.CurrentPrice <= x.Key && x.Key <= price)).OrderBy(x => x.Key).Select(x => x.Value);
                foreach (var i in inRangeBids)
                {
                    if (i.Total > 0)
                    {
                        var bids = i.Take(quantity, out var unredeemed);
                        foreach (var item in bids)
                        {
                            await _hubContext.Clients.User(item.Bidder).ReceiveRecords(await GetTradingRecords(item.Bidder));
                        }
                        s.UpdateCurrentPrice();
                        quantity = unredeemed;
                    }
                }

                if (s.Asks.ContainsKey(price))
                {
                    var collection = s.Asks[price];
                    if(quantity>0)
                    {
                        collection.Add(new Ask(userId,price,quantity));
                    }
                    
                }
                else
                {
                    s.Asks.Add(price, new AskCollection(price));
                    var collection = s.Asks[price];
                    if (quantity > 0)
                    {
                        collection.Add(new Ask(userId, quantity,quantity));
                    }
                }

            }

        }

    }
}
