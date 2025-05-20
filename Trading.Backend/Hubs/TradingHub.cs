using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Trading.Backend.Interfaces;
using Trading.Backend.Persistance;
using Trading.Backend.Services;
using Trading.Common.Models;

namespace Trading.Backend.Hubs
{
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    public class TradingHub : Hub<ITrade>
    {
        private ITradingService _service;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TradingHub> _logger;
        public TradingHub(ITradingService service, IServiceScopeFactory factory, ILogger<TradingHub> logger)
        {
            _service = service;
            _scopeFactory = factory; 
            _logger = logger;
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"{Context.ConnectionId} ({Context.UserIdentifier}) disconnected.");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendTradingHistory()
        {
            _logger.LogInformation("Context.UserIdentifier:" + Context.UserIdentifier);
            var records = _service.GetTradingRecords(Context.UserIdentifier);
            await Clients.User(Context.UserIdentifier).ReceiveRecords(records);
        }

        public async Task Order(string side, string symbol, float price, int quantity)
        {
            _logger.LogInformation("Context.UserIdentifier:" + Context.UserIdentifier);

            if(side == "Bid")
            {
                _service.Bid(Context.UserIdentifier, symbol, price, quantity);
            }
            else
            {
                _service.Ask(Context.UserIdentifier, symbol, price, quantity);
            }

            var record = new TradeRecord
            {
                Time = DateTime.UtcNow,
                Side = side,
                Ticker = symbol,
                Price = price,
                Quantity = quantity,
                UserId = Context.UserIdentifier,
            };
            _service.AddRecord(record);
        }
    }
}
