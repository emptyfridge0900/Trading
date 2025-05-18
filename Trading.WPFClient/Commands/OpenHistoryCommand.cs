using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Trading.WPFClient.ViewModels;

namespace Trading.WPFClient.Commands
{
    public class OpenHistoryCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private UIViewModel _UIViewModel;
        private Window _mainWindow;
        public OpenHistoryCommand(UIViewModel viewModel, Window mainWindow)
        {
            _UIViewModel = viewModel;
            _mainWindow = mainWindow;
        }
        public bool CanExecute(object? parameter)
        {
            return _UIViewModel.Ticker != null;
        }

        public void Execute(object? parameter)
        {
            var win = new HistoryWindow()
            {
                DataContext = new SubViewModel(new HistoryViewModel(_UIViewModel.Jwt))
            };
            win.Owner = _mainWindow;

            win.Show();
        }
    }
}
