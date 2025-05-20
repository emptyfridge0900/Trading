namespace Trading.Backend.Models
{
    /// <summary>
    /// This class represnets ask order 
    /// </summary>
    public class Ask
    {
        public string Asker { get; init; }
        public float Price { get; init; }
        public int Quantity { get; private set; }
        public Ask(string asker,float price,int quantity) 
        {
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price));
            if (quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));
            Asker = asker;
            Price = price;
            Quantity = quantity;
        }
        public void UpdateQuantity(int quantity)
        {
            if (quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity));
            Quantity = quantity;
        }
    }
    public class AskCollection
    {
        public float Price { get; init; }
        public int Total { get; private set; }

        public Queue<Ask> AskQueue { get; private set; }

        public AskCollection(float price)
        {
            Price = price;
            Total = 0;
            AskQueue = new Queue<Ask>();
        }

        public void Add(Ask ask)
        {
            Total += ask.Quantity;
            AskQueue.Enqueue(ask);
        }
        public List<Ask> Take(int quantity, out int unredeemed)
        {
            var result = new List<Ask>();
            if (AskQueue.TryPeek(out var ask))
            {
                if (ask.Quantity > quantity)
                {
                    Total -= quantity;
                    ask.UpdateQuantity(ask.Quantity - quantity);
                    unredeemed = 0;
                    result.Add(new Ask(ask.Asker, ask.Price, quantity));
                }
                else if (ask.Quantity == quantity)
                {
                    Total -= quantity;
                    unredeemed = 0;
                    result.Add(AskQueue.Dequeue());
                }
                else
                {
                    ask = AskQueue.Dequeue();
                    result.Add(ask);
                    Total -= ask.Quantity;
                    quantity -= ask.Quantity;

                    if (AskQueue.TryPeek(out ask))
                    {
                        result.AddRange(Take(quantity, out unredeemed));
                    }
                    else
                    {
                        unredeemed = quantity;
                    }

                    return result;
                }
            }
            else
            {
                unredeemed = quantity;
            }

            return result;
        }
    }


}
