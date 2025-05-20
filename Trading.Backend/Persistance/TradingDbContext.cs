using Microsoft.EntityFrameworkCore;
using Trading.Backend.Models;
using Trading.Common.Models;

namespace Trading.Backend.Persistance
{
    /// <summary>
    /// Not a real database context, in-memory db is being used here
    /// </summary>
    public class TradingDbContext : DbContext
    {
        public DbSet<TradeRecord> Records { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticker> Tickers { get; set; }
        public TradingDbContext(DbContextOptions<TradingDbContext> options) :base(options) { }

        

    }
}
