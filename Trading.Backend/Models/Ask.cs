namespace Trading.Backend.Models
{
    /// <summary>
    /// This class represnets ask order 
    /// </summary>
    public class Ask
    {
        public string Asker { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
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
        public List<Ask> Take(int quantity, out int left)
        {
            var result = new List<Ask>();
            if (AskQueue.TryPeek(out var ask))
            {
                if (ask.Quantity > quantity)
                {
                    Total -= quantity;
                    ask.Quantity -= quantity;
                    left = 0;
                    result.Add(ask);
                }
                else if (ask.Quantity == quantity)
                {
                    Total -= quantity;
                    left = 0;
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
                        result.AddRange(Take(quantity, out left));
                    }
                    else
                    {
                        left = quantity;
                    }

                    return result;
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
