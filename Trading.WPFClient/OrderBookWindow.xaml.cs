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
    /// Interaction logic for OrderBookWindow.xaml
    /// </summary>
    public partial class OrderBookWindow : Window
    {
        public OrderBookWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        { 
            var context = (SubViewModel)DataContext;
            ((OrderBookViewModel)context.CurrentViewModel).Connect();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var context = (SubViewModel)DataContext;
            ((OrderBookViewModel)context.CurrentViewModel).Disconnect();
        }
    }
}
