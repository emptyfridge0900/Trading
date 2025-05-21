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
        /// <summary>
        /// Verify whether any asking price is higher than the bidding price. 
        /// If such a case exists, remove the corresponding ask from the AskQueue 
        /// and enqueue the remaining orders into the BidQueue.
        /// </summary>
        /// <param name="userId">Uniquely identified name</param>
        /// <param name="symbol">Ticker symbol</param>
        /// <param name="price">Price to bid</param>
        /// <param name="quantity">Quantity to bid</param>
        /// <returns></returns>
        Task Bid(string userId, string symbol, float price, int quantity);

        /// <summary>
        /// Verify whether any bidding price is higher than the asking price. 
        /// If such a case exists, remove the corresponding bid from the BidQueue 
        /// and enqueue the remaining orders into the AskQueue.
        /// </summary>
        /// <param name="userId">Uniquely identified name</param>
        /// <param name="symbol">Ticker symbol</param>
        /// <param name="price">Price to ask</param>
        /// <param name="quantity">Quantity to ask</param>
        /// <returns></returns>
        Task Ask(string userId, string symbol, float price, int quantity);

    }
}
