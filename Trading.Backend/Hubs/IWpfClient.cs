using Trading.Common.Models;

namespace Trading.Backend.Hubs
{
    public interface IWpfClient
    {
        Task ReceiveTickerUpdate(List<Order> orders, CancellationToken ct = default);
        Task ReceiveTickerList(List<Ticker> tickers, CancellationToken ct = default);
    }
}
