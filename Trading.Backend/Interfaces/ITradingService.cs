using Trading.Backend.Models;
using Trading.Common.Models;

namespace Trading.Backend.Interfaces
{
    public interface ITradingService
    {
        /// <summary>
        /// Get trading records userId. 
        /// </summary>
        /// <param name="userId">UserId in here is user's name and at the same time, it's SignalR useridentifier</param>
        /// <returns></returns>
        List<TradeRecord> GetTradingRecords(string userId);
        void AddRecord(TradeRecord record);
        void Bid(string userId, string symbol, float price, int quantity);
        void Ask(string userId, string symbol, float price, int quantity);

    }
}
