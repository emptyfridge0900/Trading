using Microsoft.AspNetCore.SignalR;
using Trading.Backend.Hubs;
using Trading.Backend.Interfaces;
using Trading.Backend.Models;

namespace Trading.Backend.Services
{
    /// <summary>
    /// Send updated orderbook info to all subscriber
    /// </summary>
    public class PriceUpdateService : BackgroundService
    {
        private readonly IHubContext<TickerHub,ITicker> _hubContext;
        private readonly ITickerService _tickerService;
        private readonly ILogger<PriceUpdateService> _logger;
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(2);

        public PriceUpdateService(
            IHubContext<TickerHub,ITicker> hubContext,
            ITickerService tickerService,
            ILogger<PriceUpdateService> logger)
        {
            _hubContext = hubContext;
            _tickerService = tickerService;
            _logger = logger;

        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _logger.LogInformation("PriceUpdateService started.");
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    //_tickerService.UpdatePrices();
                    var tickers= _tickerService.GetTickers(1, 10).Results;
                    foreach (var ticker in tickers)
                    {
                        await _hubContext.Clients.Group(ticker.Symbol).ReceiveOrderBook(_tickerService.GetOrders(ticker.Symbol,10),ct);
                        //_logger.LogInformation($"Sent ticker {ticker.Symbol}");
                    }
                    
                    
                    await Task.Delay(_updateInterval, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in PriceUpdateService.");
                }
            }
            _logger.LogInformation("PriceUpdateService stopped.");
        }
    }
}
