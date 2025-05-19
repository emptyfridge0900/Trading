using Trading.Common.Models;

namespace Trading.Backend.Interfaces
{
    public interface ITickerService
    {
        public List<Ticker> GetTickers();
        public List<Order> GetOrders(string tickerName, int numOfRow);
        public void UpdatePrices();
    }
}
