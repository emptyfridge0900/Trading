using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.WPFClient.Models
{
    public record Layer
    {
        public string Name { get; set; }
        public float Price { get; init; }
        public int Quantity { get; set; }

        public Layer(Quote quote, int index, float price, int quantity)
        {
            Name = quote.ToString() + " " + (index+1).ToString();
            Price = price;
            Quantity = quantity;
        }
    }
}
