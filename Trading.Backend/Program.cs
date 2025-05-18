
using Trading.Backend.Hubs;
using Trading.Backend.Services;

namespace Trading.Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<TradingDbContext>(options =>
            {
                options.UseInMemoryDatabase("inmemory");
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<TickerService>();
            builder.Services.AddHostedService<PriceUpdateService>();
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
                dbContext.Database.EnsureDeleted();
                dbContext.Tickers.AddRange(new List<Ticker>{
                        new Ticker { Symbol = "AAPL", Price = 230.45f },
                        new Ticker { Symbol = "MSFT", Price = 420.10f },
                        new Ticker { Symbol = "GOOGL", Price = 175.20f },
                        new Ticker { Symbol = "AMZN", Price = 190.50f },
                        new Ticker { Symbol = "TSLA", Price = 340.75f },
                        new Ticker { Symbol = "NVDA", Price = 135.30f },
                        new Ticker { Symbol = "META", Price = 510.25f },
                        new Ticker { Symbol = "JPM", Price = 205.15f },
                        new Ticker { Symbol = "V", Price = 280.90f },
                        new Ticker { Symbol = "WMT", Price = 75.60f }
                    });
                dbContext.SaveChanges();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.MapHub<TickerHub>("/trading");
            app.Run();
        }
    }
}
