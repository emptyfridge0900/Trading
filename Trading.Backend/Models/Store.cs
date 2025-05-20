using Trading.Common.Models;

namespace Trading.Backend.Models
{
    /// <summary>
    /// Simulate stock information in a database
    /// </summary>
    public class Store
    {
        public Dictionary<string,Stock> Stocks { get; init; }
        public Store() 
        { 
            Stocks = new Dictionary<string, Stock>()
            {
                { "AAPL", new Stock("AAPL",230.45f) },
                { "MSFT", new Stock("MSFT",420.10f) },
                { "GOOGL", new Stock("GOOGL",175.20f) },
                { "AMZN", new Stock("AMZN",190.50f) },
                { "TSLA", new Stock("TSLA",340.75f) },
                { "NVDA", new Stock("NVDA",135.30f) },
                { "META", new Stock("META",510.25f) },
                { "JPM", new Stock("JPM",205.15f) },
                { "V", new Stock("V",280.90f) },
                { "WMT", new Stock("WMT",75.60f) }
            };
        }
    }
}
