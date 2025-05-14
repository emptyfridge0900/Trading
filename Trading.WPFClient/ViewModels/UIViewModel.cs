using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Trading.WPFClient.Commands;
using Trading.WPFClient.Models;
using Trading.WPFClient.Services;
using Trading.WPFClient.Views;

namespace Trading.WPFClient.ViewModels
{
    public class UIViewModel:ViewModelBase
    {
        private Window _mainWindow;
        public ObservableCollection<Ticker> Tickers { get; set; }

        private Ticker _ticker;
        public Ticker Ticker
        {
            get { return _ticker; }
            set { 
                _ticker = value; 
                OnPropertyChanged(nameof(Ticker));
            }
        }
        public UIViewModel(Window mainWinow)
        {
            _mainWindow = mainWinow;
            Tickers = new ObservableCollection<Ticker> { new Ticker
            {
                Name="test",
                LastPrice =10,
                ChangePercent=11,
                Changes=9
            } };
            OpenHistory = new OpenOrderBookCommand(this,_mainWindow);
            OpenOrder = new OpenOrderBookCommand(this,_mainWindow);


            
        }
        public ICommand OpenHistory { get; set; }
        public ICommand OpenOrder { get; set; }


    }
}
