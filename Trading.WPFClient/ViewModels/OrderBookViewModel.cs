using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Trading.Common.Models;
using Trading.WPFClient.Models;

namespace Trading.WPFClient.ViewModels
{
    public class OrderBookViewModel: ViewModelBase
    {
        private readonly HubConnection _hubConnection;
        private string _tickerName;
        public string TickerName
        {
            get { return _tickerName; }
            set { 
                _hubConnection.InvokeAsync("UnjoinGroup", _tickerName);
                _tickerName = value;
                OnPropertyChanged(nameof(TickerName));
                _hubConnection.InvokeAsync("JoinGroup", _tickerName);
            }
        }

        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders));
            }
        }

        private ObservableCollection<string> _tickers;
        public ObservableCollection<string> Tickers
        {
            get { return _tickers; }
            set
            {
                _tickers = value;
                OnPropertyChanged(nameof(Tickers));
            }
        }

        public OrderBookViewModel(string symbol, List<Ticker> tickers)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7004/trading")
                .Build();
            
            _hubConnection.On("ReceiveTickerUpdate", (List<Order> x) => {

                Orders = new ObservableCollection<Order>(x);

            }); 
            TickerName = symbol;
            Tickers = new ObservableCollection<string>(tickers.Select(x=>x.Symbol));
        }

        public async void Connect()
        {
            await _hubConnection.StartAsync();
            await _hubConnection.InvokeAsync("JoinGroup", TickerName);
        }
        private async void SelectTicker(string tickerName)
        {
            await _hubConnection.SendAsync("JoinGroup", tickerName);
        }
    }
}
