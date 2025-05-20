
using System;
using Trading.Backend.Interfaces;
using Trading.Backend.Persistance;

namespace Trading.Backend.Services
{
    /// <summary>
    /// Data seeder
    /// </summary>
    public class DataProducer : IHostedService
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
        public Task StartAsync(CancellationToken ct)
        {
            _logger.LogInformation("Data producer started.");
            using var scope = _scopeFactory.CreateScope();
            var tickers = _tickerService.GetTickers(1, 10).Results;
            Random rnd = new Random();
            foreach (var ticker in tickers)
            {
                int j = 0;
                for (int i = 1; i < 20; i++)
                {
                    _tradingService.Bid(_userName, ticker.Symbol,ticker.Price+i, rnd.Next(1, 10));
                    _tradingService.Ask(_userName, ticker.Symbol, ticker.Price+j, rnd.Next(1, 10));
                    j--;
                }
            }
            //while (!ct.IsCancellationRequested)
            //{


            //}

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Data producer stopped.");
            return Task.CompletedTask;
        }
    }
}
