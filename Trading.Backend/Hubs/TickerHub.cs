using Microsoft.AspNetCore.SignalR;
using Trading.Backend.Interfaces;
using Trading.Backend.Services;

namespace Trading.Backend.Hubs
{
    public class TickerHub : Hub<IWpfClient>
    {
        private readonly ITickerService _tickerService;
        private readonly ILogger<TickerHub> _logger;
        private readonly UserService _userService;
        public TickerHub(ITickerService tickerService, ILogger<TickerHub> logger, UserService userService) 
        {
            _tickerService = tickerService;
            _logger = logger;
            _userService = userService;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"{Context.ConnectionId} disconnected.");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendTickerLit()
            => await Clients.Caller.ReceiveTickerList(_tickerService.GetTickers());
        
        public async Task JoinGroup(string tickerName)
         => await Groups.AddToGroupAsync(Context.ConnectionId, tickerName);
        
        public async Task UnjoinGroup(string tickerName)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, tickerName);
        
        public async Task Login(string name)
        {
            _userService.CreateUser(name);
        }
             
    }
}
