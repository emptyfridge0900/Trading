using Trading.Common.Models;

namespace Trading.Backend.Interfaces
{
    public interface ITickerService
    {
        public Task<PaginatedResult<Ticker>> GetTickers(int pageNum, int pageSize);
        public Task<List<Order>> GetOrders(string tickerName, int numOfRow);
    }
}
