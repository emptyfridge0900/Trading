using Microsoft.AspNetCore.SignalR;
using Trading.Backend.Services;
using Trading.Common.Models;

namespace Trading.Backend.Hubs
{
    public class TradingHub : Hub<IWpfClient>
    {
        private readonly TickerService _tickerService;
        public TradingHub(TickerService tickerService) 
        {
            _tickerService = tickerService;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"{Context.ConnectionId} disconnected.");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendTickerLit()
            => await Clients.Caller.ReceiveTickerList(_tickerService.GetTickers());
        
        public async Task JoinGroup(string tickerName)  
            => await Groups.AddToGroupAsync(Context.ConnectionId, tickerName);
        
        public async Task UnjoinGroup(string tickerName)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, tickerName);
        
    }
}
