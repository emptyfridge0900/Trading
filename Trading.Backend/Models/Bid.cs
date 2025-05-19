namespace Trading.Backend.Models
{
    public class Bid
    {
        public string Bidder { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
    }
    public class BidCollection
    {
        public float Price { get; init; }
        public int Total { get; private set; }

        public Queue<Bid> BidQueue { get; private set; }

        public BidCollection(float price)
        {
            Price = price;
            Total = 0;
            BidQueue = new Queue<Bid>();
        }
        public void Add(Bid bid)
        {
            Total += bid.Quantity;
            BidQueue.Enqueue(bid);
        }

        public List<Bid> Take(int quantity, out int left)
        {
            var result = new List<Bid>();
            if (BidQueue.TryPeek(out var bid))
            {
                if (bid.Quantity > quantity)
                {
                    Total -= quantity;
                    bid.Quantity -= quantity;
                    left = 0;
                    result.Add(bid);
                }
                else if (bid.Quantity == quantity)
                {
                    Total -= quantity;
                    left = 0;
                    result.Add(BidQueue.Dequeue());
                }
                else
                {
                    bid = BidQueue.Dequeue();
                    result.Add(bid);
                    Total -= quantity;
                    quantity -= bid.Quantity;

                    if (BidQueue.TryPeek(out bid))
                    {
                        result.AddRange(Take(quantity, out left));
                    }
                    else
                    {
                        left = quantity;
                    }

                }
            }
            else
            {
                left = quantity;
            }

            return result;
        }
    }

}
