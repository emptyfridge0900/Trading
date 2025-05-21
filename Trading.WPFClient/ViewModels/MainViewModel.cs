using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Trading.WPFClient.ViewModels
{
    class MainViewModel:ViewModelBase
    {
        public ViewModelBase CurrentViewModel { get; set; }
        public MainViewModel()
        {
            CurrentViewModel = new UIViewModel();
        }
    }
}
