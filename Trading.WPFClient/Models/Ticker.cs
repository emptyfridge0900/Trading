using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.WPFClient.Models
{
    public class Ticker
    {
        public string Name {  get; set; }
        public float LastPrice {  get; set; }
        public float Changes {  get; set; }
        public float ChangePercent {  get; set; }
    }
}
