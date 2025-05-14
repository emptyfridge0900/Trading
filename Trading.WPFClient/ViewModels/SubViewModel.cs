using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Trading.WPFClient.ViewModels
{
    public class SubViewModel: ViewModelBase
    {
        public ViewModelBase CurrentViewModel { get; set; }
        public SubViewModel(ViewModelBase viewModel)
        {
            CurrentViewModel = viewModel;
        }
    }
}
