using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Trading.WPFClient.ViewModels;

namespace Trading.WPFClient
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
        {
            InitializeComponent();
        }

        //https://www.youtube.com/watch?v=n3ZR9NlbB1E&t=168s
        private void Window_Closed(object sender, EventArgs e)
        {
            var context = (SubViewModel)DataContext;
            ((HistoryViewModel)context.CurrentViewModel).Disconnect();
        }

        /// <summary>
        /// Calling Connect() in constructor of viewmodel doesn't work
        /// Call it once window is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var context = (SubViewModel)DataContext;
            ((HistoryViewModel)context.CurrentViewModel).Connect();
        }
    }
}
