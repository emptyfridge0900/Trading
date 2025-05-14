using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.WPFClient.Models
{
    public class OrderBook
    {
        public string TickerName {  get; set; }
        public List<Layer> Layers { get; private set; }
        public OrderBook(string name)
        {
            TickerName = name;
            Layers = new List<Layer>();
        }
    }
}
