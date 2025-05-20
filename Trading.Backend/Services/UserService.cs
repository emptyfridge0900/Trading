using Microsoft.EntityFrameworkCore;
using Trading.Backend.Models;
using Trading.Backend.Persistance;
using Trading.Common.Models;

namespace Trading.Backend.Services
{
    public class UserService
    {
 
        private readonly IServiceScopeFactory _scopeFactory;
        public UserService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public User CreateUser(string userId)
        {
            var records = new List<TradeRecord>();
            var datetimes = RandomDateTimes(5);
            using var db = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<TradingDbContext>();
            var user = db.Users.Where(u => u.UserId == userId).Include(u => u.TradeRecords).SingleOrDefault();

            var tickers = db.Tickers;

            foreach (var datetime in datetimes)
            {
                records.Add(new TradeRecord
                {
                    Time = datetime,
                    Side = new[] { "Bid", "Ask" }[new Random().Next(0, 2)],
                    Ticker = tickers.Select(x => x.Symbol).ToList()[new Random().Next(0, tickers.Count())],
                    Price = new Random().Next(1, 99),
                    Quantity = new Random().Next(1, 99),
                    UserId = userId                
                });
            }

            if (user == null)
            {
                user = new User
                {
                    UserId = userId,
                    Name = userId,
                    TradeRecords = records
                };
                db.Users.Add(user);
                db.SaveChanges();
            }
            return user;
        }

        private List<DateTime> RandomDateTimes(int numberOfDates)
        {
            DateTime start = new DateTime(2020, 1, 1);
            DateTime end = new DateTime(2025, 5, 1);

            Random rand = new Random();

            List<DateTime> randomDates = new List<DateTime>();

            for (int i = 0; i < numberOfDates; i++)
            {
                TimeSpan range = end - start;
                TimeSpan randomTimeSpan = new TimeSpan((long)(rand.NextDouble() * range.Ticks));

                DateTime randomDate = start + randomTimeSpan;
                randomDates.Add(randomDate);
            }

            // Sort the list in ascending order
            var sortedDates = randomDates.OrderBy(d => d).ToList();


            return sortedDates;
        }

    }
}
