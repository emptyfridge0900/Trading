using Trading.Common.Models;

namespace Trading.Backend.Interfaces
{
    public interface ITicker
    {
        Task ReceiveOrderBook(List<Order> orders, CancellationToken ct = default);
        Task ReceiveTickerList(PaginatedResult<Ticker> tickers, CancellationToken ct = default);
    }
}
