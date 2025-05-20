using Trading.Common.Models;

namespace Trading.Backend.Hubs
{
    public interface IWpfClient
    {
        Task ReceiveTickerUpdate(List<Order> orders, CancellationToken ct = default);
        Task ReceiveTickerList(PaginatedResult<Ticker> tickers, CancellationToken ct = default);
        Task ReceiveJwt(string jwt, CancellationToken ct = default);
    }
}
