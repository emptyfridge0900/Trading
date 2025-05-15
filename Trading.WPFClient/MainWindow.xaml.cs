using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Trading.WPFClient.ViewModels;

namespace Trading.WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ContentControl_Loaded(object sender, RoutedEventArgs e)
        {
            var context = (MainViewModel)DataContext;
            ((UIViewModel)context.CurrentViewModel).Connect();
        }
    }
}