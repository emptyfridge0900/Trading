using Trading.Common.Models;

namespace Trading.Backend.Interfaces
{
    public interface ITickerService
    {
        public PaginatedResult<Ticker> GetTickers(int pageNum, int pageSize);
        public List<Order> GetOrders(string tickerName, int numOfRow);
    }
}
