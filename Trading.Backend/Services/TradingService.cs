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
        public TradingService(IServiceScopeFactory factory, ILogger<TradingService> logger, 
            IHubContext<TradingHub, ITrade> hubContext, ITickerService tickerService, Store store)
        {
            _scopeFactory = factory;
            _logger = logger;
            _store = store;
            _hubContext = hubContext;
            _tickerService = tickerService;
        }
        private List<DateTime> RandomDateTimes(int numberOfDates)
        {
            DateTime start = new DateTime(2020, 1, 1);
            DateTime end = new DateTime(2025, 5, 1);

            Random rand = new Random();

            List<DateTime> randomDates = new List<DateTime>();

            for (int i = 0; i<numberOfDates; i++)
            {
                TimeSpan range = end - start;
                TimeSpan randomTimeSpan = new TimeSpan((long)(rand.NextDouble() * range.Ticks));

                DateTime randomDate = start + randomTimeSpan;
                randomDates.Add(randomDate);
            }

            // Sort the list in ascending order
            var sortedDates = randomDates.OrderByDescending(d => d).ToList();


            return sortedDates;
        }
        private List<TradeRecord> HandleEmptyRecord(string userId)
        {
            var records = new List<TradeRecord>();
            var datetimes = RandomDateTimes(5);
            using var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TradingDbContext>();
            var tickers = db.Tickers;

            foreach (var datetime in datetimes)
            {
                records.Add(new TradeRecord
                {
                    Time = datetime,
                    Side = new []{ "Bid", "Ask" }[new Random().Next(0, 2)],
                    Ticker = tickers.Select(x => x.Symbol).ToList()[new Random().Next(0, tickers.Count())],
                    Price = new Random().Next(1,99),
                    Quantity = new Random().Next(1,99),
                    UserId = userId
                });
            }
            
            return records;
        }
        
        public List<TradeRecord> GetTradingRecords(string userId)
        {
            using var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TradingDbContext>();
            var user = db.Users.Where(u=>u.UserId == userId).Include(u=>u.TradeRecords).SingleOrDefault();

            if (user == null)
            {
                user = new Models.User
                {
                    UserId = userId,
                    Name = userId,
                    TradeRecords = HandleEmptyRecord(userId),
                };
                db.Users.Add(user);
                db.SaveChanges();
            }
            else if (user.TradeRecords.Count == 0)
            {
                user.TradeRecords.AddRange(HandleEmptyRecord(userId));
                db.SaveChanges();
            }
            return user.TradeRecords;
        }

        public void AddRecord(string userId, TradeRecord record)
        {
            using var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TradingDbContext>();
            var user = db.Users.Where(u => u.UserId == userId).Include(u => u.TradeRecords).SingleOrDefault();
            if (user == null)
            {
                user = new User
                {
                    UserId = userId,
                    Name = userId,
                    TradeRecords = HandleEmptyRecord(userId),
                };
                db.Users.Add(user);
                db.SaveChanges();
            }
            user.TradeRecords.Add(record);
            db.SaveChanges();
        }
        public void Bid(string userId, string symbol, float price, int quantity)
        {
            if (_store.Stocks.ContainsKey(symbol))
            {
                var s = _store.Stocks[symbol];
                s.UpdateCurrentPrice();
                //before starting bid, check the bid price is in ask price
                var inRangeAsks = s.Asks.Where(x => (s.CurrentPrice >= x.Key && x.Key >= price)).OrderByDescending(x=>x.Key).Select(x=>x.Value);
                foreach (var i in inRangeAsks)
                {
                    if (i.Total > 0)
                    {
                        var asks = i.Take(quantity, out var remain);
                        foreach (var item in asks)
                        {
                            _hubContext.Clients.User(item.Asker).ReceiveRecords(GetTradingRecords(item.Asker));
                        }
                        s.UpdateCurrentPrice();
                        quantity = remain;
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

                if (s.Bids.ContainsKey(price))
                {
                    var collection = s.Bids[price];
                    if(quantity > 0)
                    {
                        collection.Add(new Bid
                        {
                            Bidder = userId,
                            Price = price,
                            Quantity = quantity,
                        });
                    }
                    
                }
                else
                {
                    s.Bids.Add(price, new BidCollection(price));
                    var collection = s.Bids[price];
                    if (quantity > 0)
                    {
                        collection.Add(new Bid
                        {
                            Bidder = userId,
                            Price = price,
                            Quantity = quantity,
                        });
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
                        var bids = i.Take(quantity, out var remain);
                        foreach (var item in bids)
                        {
                            _hubContext.Clients.User(item.Bidder).ReceiveRecords(GetTradingRecords(item.Bidder));
                        }
                        s.UpdateCurrentPrice();
                        quantity = remain;
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
                        collection.Add(new Ask
                        {
                            Asker = userId,
                            Price = price,
                            Quantity = quantity,
                        });
                    }
                    
                }
                else
                {
                    s.Asks.Add(price, new AskCollection(price));
                    var collection = s.Asks[price];
                    if (quantity > 0)
                    {
                        collection.Add(new Ask
                        {
                            Asker = userId,
                            Price = price,
                            Quantity = quantity,
                        });
                    }
                }

            }

        }

    }
}
