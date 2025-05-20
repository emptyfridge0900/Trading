using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Trading.WPFClient.Models;
using Trading.WPFClient.ViewModels;

namespace Trading.WPFClient.Commands
{
    public class OpenOrderBookCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        /// <summary>
        /// ViewModel is here for passing ticker information to OrderBookViewModel.
        /// There are better way to solve it, but it's work. Just leave it for now
        /// </summary>
        private UIViewModel _UIViewModel;
        public OpenOrderBookCommand(UIViewModel viewModel)
        {
            _UIViewModel = viewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return _UIViewModel.Ticker!=null;
        }

        public void Execute(object? parameter)
        {
            var win = new OrderBookWindow()
            {
                DataContext = new SubViewModel(new OrderBookViewModel(_UIViewModel.Ticker.Symbol, _UIViewModel.Tickers.ToList()))
            };


            win.Show();

        }
    }
}
