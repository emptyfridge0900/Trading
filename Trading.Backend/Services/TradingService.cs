using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Trading.Backend.Hubs;
using Trading.Backend.Interfaces;
using Trading.Backend.Persistance;
using Trading.Common.Models;

namespace Trading.Backend.Services
{
    public class TradingService:ITradingService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TradingService> _logger;
        public TradingService(IServiceScopeFactory factory, ILogger<TradingService> logger)
        {
            _scopeFactory = factory;
            _logger = logger;
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
            
            if(user == null)
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
            return user.TradeRecords;
        }

        public void AddRecord(string userId, TradeRecord record)
        {
            using var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TradingDbContext>();
            var user = db.Users.Where(u => u.UserId == userId).Include(u => u.TradeRecords).SingleOrDefault();
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
            user.TradeRecords.Add(record);
        }



    }
}
