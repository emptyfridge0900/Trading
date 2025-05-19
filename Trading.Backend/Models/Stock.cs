using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Trading.Backend.Services;

namespace Trading.Backend.Models
{
    public class Stock
    {
        public string Name {  get; set; }
        public float CurrentPrice {  get; set; }
        public Dictionary<float, BidCollection> Bids { get; set; }
        public Dictionary<float, AskCollection> Asks { get; set; }
        public Stock(string name,float price) 
        {
            Name = name;
            CurrentPrice = price;
            Bids =new Dictionary<float, BidCollection>();
            Asks=new Dictionary<float, AskCollection>();
        }

        //lowest bid price will be the current price
        public void UpdateCurrentPrice()
        {
            var bids = Bids.OrderBy(x => x.Key).Select(x => x.Value);
            foreach (var x in bids)
            {
                if (x.Total > 0)
                {
                    CurrentPrice = x.Price;
                    return;
                }
            }
        }


    }
}
