using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.WPFClient.Models
{
    public record TradeHistory
    {
        public DateTime Time { get; set; }
        public string Side {  get; set; }
        public string Ticker {  get; set; }
        public float Price {  get; set; }
        public int Quantity {  get; set; }
        public float Total {  get; set; }
    }
}
