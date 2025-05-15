using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Trading.WPFClient.Commands;
using Trading.Common.Models;
using Trading.WPFClient.Services;
using Trading.WPFClient.Views;

namespace Trading.WPFClient.ViewModels
{
    public class UIViewModel:ViewModelBase
    {
        private readonly HubConnection _hubConnection;

        private Window _mainWindow;
        private ObservableCollection<Ticker> _tickers;
        public ObservableCollection<Ticker> Tickers 
        { 
            get { return _tickers; }
            set
            {
                _tickers = value;
                OnPropertyChanged(nameof(Tickers));
            } 
        }

        private Ticker _ticker;
        public Ticker Ticker
        {
            get { return _ticker; }
            set { 
                _ticker = value; 
                OnPropertyChanged(nameof(Ticker));
                _hubConnection.InvokeAsync("TickerSelected", _ticker);
            }
        }
        public UIViewModel(Window mainWinow)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7004/trading")
                .Build();
            _hubConnection.On("ReceiveTickerList", (List<Ticker> x) => {
                Tickers = new ObservableCollection<Ticker>(x);
            });
            _mainWindow = mainWinow;

            OpenHistory = new OpenOrderBookCommand(this,_mainWindow);
            OpenOrder = new OpenOrderBookCommand(this,_mainWindow);
        }
        public async void Connect()
        {
            await _hubConnection.StartAsync();
            await _hubConnection.InvokeAsync("SendTickerLit");
        }
        public ICommand OpenHistory { get; set; }
        public ICommand OpenOrder { get; set; }


    }
}
