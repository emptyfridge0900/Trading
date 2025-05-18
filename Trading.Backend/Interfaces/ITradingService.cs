using Trading.Common.Models;

namespace Trading.Backend.Interfaces
{
    public interface ITradingService
    {
        public List<TradeRecord> GetTradingRecords(string userId);
        public void AddRecord(string userId, TradeRecord record);
    }
}
