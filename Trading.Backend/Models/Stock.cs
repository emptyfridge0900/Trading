namespace Trading.Backend.Models
{
    public class Stock
    {
        public string Name {  get; init; }
        public float CurrentPrice {  get; private set; }
        public SortedDictionary<float, BidCollection> Bids { get; init; }
        public SortedDictionary<float, AskCollection> Asks { get; init; }
        public Stock(string name,float price) 
        {
            Name = name;
            CurrentPrice = price;
            //sort in ascending order
            Bids = new SortedDictionary<float, BidCollection>();
            //descending order
            Asks = new SortedDictionary<float, AskCollection>(Comparer<float>.Create((x, y) => y.CompareTo(x)));
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
