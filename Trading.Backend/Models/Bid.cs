namespace Trading.Backend.Models
{
    /// <summary>
    /// This class represents Bid order
    /// </summary>
    public class Bid
    {
        public string Bidder { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
    }
    public class BidCollection
    {
        public float Price { get; init; }

        /// <summary>
        /// Bid quantity in a certain price
        /// </summary>
        public int Total { get; private set; }

        /// <summary>
        /// A queue where bids of the same price are grouped in order.
        /// </summary>
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
        /// <summary>
        /// Returns bids for the quantity you entered
        /// </summary>
        /// <param name="quantity">how many bid to remove</param>
        /// <param name="left">remaining quantity</param>
        /// <returns></returns>
        public List<Bid> Take(int quantity, out int left)
        {
            var result = new List<Bid>();

            //check if the queue is not empty
            if (BidQueue.TryPeek(out var bid))
            {
                //When the quantity to be removed is less than the bid quantity,
                //dequeue is not performed because there is still quantity remaining.
                if (bid.Quantity > quantity)
                {
                    Total -= quantity;
                    bid.Quantity -= quantity;
                    left = 0;
                    result.Add(bid);
                }
                //removing quantity and bid quantity are same.
                //no biding left, dequeue the bid and 0 remaining
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
                    //quantity to remove exceeds bid quantity. see if there is more bid with same price.
                    //if more biding with same price, then take from there
                    //if no bidding left, then returns remaining number
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
