using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.WPFClient.Models;

namespace Trading.WPFClient.ViewModels
{
    public class OrderBookViewModel: ViewModelBase
    {
        private string _tickerName;
        public string TickerName
        {
            get { return _tickerName; }
            set { 
                _tickerName = value;
                OnPropertyChanged(nameof(TickerName));
            }
        }
        private ObservableCollection<Layer> _layer;
        public IEnumerable<Layer> Layers => _layer;
        public OrderBookViewModel()
        {
            _tickerName = "Test123";
            _layer = new ObservableCollection<Layer> { 
                new Layer(Quote.Bid, 0, 11, 10),
                new Layer(Quote.Bid, 1, 12, 14),
                new Layer(Quote.Bid, 2, 13, 11),
                new Layer(Quote.Bid, 3, 15, 10)
            };
        }
    }
}
