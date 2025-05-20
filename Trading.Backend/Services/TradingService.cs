using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
        private readonly ITickerService _tickerService;
        private readonly UserService _userService;
        public TradingService(IServiceScopeFactory factory, ILogger<TradingService> logger, 
            IHubContext<TradingHub, ITrade> hubContext, ITickerService tickerService, Store store, UserService userService)
        {
            _scopeFactory = factory;
            _logger = logger;
            _store = store;
            _hubContext = hubContext;
            _tickerService = tickerService;
            _userService = userService;
        }
        public List<TradeRecord> GetTradingRecords(string userId)
        {
            using var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TradingDbContext>();
            var records = db.Records.Where(r => r.UserId == userId);
            //there is 0 possibility that a user doesn't have any records. Because when a user is created, random records are given

            return records.Reverse().ToList();
            
        }

        public void AddRecord(TradeRecord record)
        {
            using var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TradingDbContext>();
            //assume that all the user inputs are valid
            db.Records.Add(record);
            
            db.SaveChanges();
        }
        public void Bid(string userId, string symbol, float price, int quantity)
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
                            _hubContext.Clients.User(item.Asker).ReceiveRecords(GetTradingRecords(item.Asker));
                        }
                        s.UpdateCurrentPrice();
                        quantity = unredeemed;
                    }
                }
                //if (s.Asks.ContainsKey(price))
                //{
                //    var collection = s.Asks[price];
                //    var asks = collection.Take(quantity,out var remain);
                //    foreach (var item in asks)
                //    {
                //        _hubContext.Clients.User(item.Asker).ReceiveRecords(GetTradingRecords(item.Asker));
                //    }
                //    quantity = remain;
                //}


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
        public void Ask(string userId, string symbol, float price, int quantity)
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
                            _hubContext.Clients.User(item.Bidder).ReceiveRecords(GetTradingRecords(item.Bidder));
                        }
                        s.UpdateCurrentPrice();
                        quantity = unredeemed;
                    }
                }
                //if (s.Bids.ContainsKey(price))
                //{
                //    var collection = s.Bids[price];
                //    var asks = collection.Take(quantity, out var remain);
                //    foreach (var item in asks)
                //    {
                //        _hubContext.Clients.User(item.Bidder).ReceiveRecords(GetTradingRecords(item.Bidder));
                //    }
                //    quantity = remain;
                //}
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
