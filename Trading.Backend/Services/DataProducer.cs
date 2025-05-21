using Trading.Backend.Interfaces;
using Trading.Common.Models;

namespace Trading.Backend.Services
{
    /// <summary>
    /// Data seeder
    /// </summary>
    public class DataProducer : BackgroundService
    {
        private readonly ITradingService _tradingService;
        private readonly ITickerService _tickerService;
        private ILogger<DataProducer> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        static string _userName = "Doosan";
        public DataProducer(ITradingService tradingService, ILogger<DataProducer> logger, 
            IServiceScopeFactory scopeFactory, ITickerService tickerService)
        {
            _tradingService = tradingService;
            _logger = logger;
            _scopeFactory = scopeFactory;
            _tickerService = tickerService;
        }
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("Data producer started.");
            using (var scope = _scopeFactory.CreateScope())
            {
                var tickers = (await _tickerService.GetTickers(1, 10)).Results;
                Random rnd = new Random();
                foreach (var ticker in tickers)
                {
                    int j = 0;
                    for (int i = 1; i < 20; i++)
                    {
                        await _tradingService.Bid(_userName, ticker.Symbol, ticker.Price + i, rnd.Next(1, 10));
                        await _tradingService.Ask(_userName, ticker.Symbol, ticker.Price + j, rnd.Next(1, 10));
                        j--;
                    }
                }
            }
            _logger.LogInformation("Data producer seeded data");

            await ScalpingTrade(ct);

            await Task.CompletedTask;
        }
        private async Task ScalpingTrade(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var tickers = (await _tickerService.GetTickers(1, 10)).Results;
                    Random rnd = new Random();
                    var ticker = tickers[rnd.Next(1, 10)];
                    var rndPrice = rnd.Next(-5, 5);
                    var bid = rnd.Next(0, 2);
                    //_logger.LogInformation($"{ticker.Symbol}:{ticker.Price + rndPrice}");

                    if (bid == 0)
                        await _tradingService.Bid(_userName, ticker.Symbol, ticker.Price + rndPrice, rnd.Next(1, 10));
                    else
                        await _tradingService.Ask(_userName, ticker.Symbol, ticker.Price + rndPrice, rnd.Next(1, 10));
                }
                await Task.Delay(100, ct);
            }

        }
    }
}
