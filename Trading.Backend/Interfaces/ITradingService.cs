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
        Task<List<TradeRecord>> GetTradingRecords(string userId);
        Task AddRecord(TradeRecord record);
        Task Bid(string userId, string symbol, float price, int quantity);
        Task Ask(string userId, string symbol, float price, int quantity);

    }
}
