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
        public OpenHistoryCommand(UIViewModel viewModel)
        {
            _UIViewModel = viewModel;
        }
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            var win = new HistoryWindow()
            {
                DataContext = new SubViewModel(new HistoryViewModel(_UIViewModel.Jwt))
            };

            win.Show();
        }
    }
}
