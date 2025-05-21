using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Trading.WPFClient.Commands
{
    /// <summary>
    /// RelayCommand
    /// </summary>
    public class DefaultCommand : ICommand
    {
        readonly Func<Task> _exe;
        readonly Predicate<object?>? _canExe;
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public DefaultCommand(Func<Task> exe, Predicate<object?>? canExe)
        {
            _exe = exe;
            _canExe = canExe;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExe == null ? true : _canExe(parameter);
        }

        public void Execute(object? parameter)
        {
            Task.Run(async () => await _exe());
        }
    }
}
