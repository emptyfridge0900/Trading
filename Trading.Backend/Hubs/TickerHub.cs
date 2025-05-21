using Microsoft.AspNetCore.SignalR;
using Trading.Backend.Interfaces;
using Trading.Backend.Services;

namespace Trading.Backend.Hubs
{
    public class TickerHub : Hub<ITicker>
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

        //just logging purpose
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"{Context.ConnectionId} ({Context.UserIdentifier}) disconnected.");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendTickerList(int pageNum, int pageSize)
            => await Clients.Caller.ReceiveTickerList(_tickerService.GetTickers(pageNum, pageSize));
        
        public async Task JoinGroup(string tickerName)
         => await Groups.AddToGroupAsync(Context.ConnectionId, tickerName);
        
        public async Task UnjoinGroup(string tickerName)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, tickerName);

        /// <summary>
        /// If the client sends their name, we will consider them logged in.
        /// </summary>
        /// <param name="name">name of the client, this name matches SignalR' user identifier</param>
        /// <returns>return nothing, client has own jwt generator</returns>
        public async Task Login(string name)
        {
            await _userService.CreateUser(name);
        }
             
    }
}
