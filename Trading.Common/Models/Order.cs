namespace Trading.Common.Models
{
    public class Order
    {
        public string Name {  get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public Order(string name, float price, int quantity)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (price < 0) throw new ArgumentOutOfRangeException(nameof(price));
            if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            Name = name;
            Price = price;
            Quantity = quantity;
        }
    }
}
