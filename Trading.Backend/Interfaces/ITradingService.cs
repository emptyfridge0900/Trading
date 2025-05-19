using Trading.Backend.Models;
using Trading.Common.Models;

namespace Trading.Backend.Interfaces
{
    public interface ITradingService
    {
        List<TradeRecord> GetTradingRecords(string userId);
        void AddRecord(string userId, TradeRecord record);
        void Bid(string userId, string symbol, float price, int quantity);
        void Ask(string userId, string symbol, float price, int quantity);

    }
}
