using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Trading.Backend.Models;
using Trading.Common.Models;

namespace Trading.Backend.Persistance
{
    public class TradingDbContext : DbContext
    {
        public DbSet<TradeRecord> Records { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Ticker> Tickers { get; set; }
        public TradingDbContext(DbContextOptions<TradingDbContext> options) :base(options) { }

        

    }
}
