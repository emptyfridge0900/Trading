﻿using System.Text;
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



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var context = (MainViewModel)DataContext;
            Task.WhenAll(new[] {
                ((UIViewModel)context.CurrentViewModel).Connect(),
                ((UIViewModel)context.CurrentViewModel).TradeHubConnect()
            });

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var context = (MainViewModel)DataContext;
            Task.WhenAll(new[] {
                ((UIViewModel)context.CurrentViewModel).Disconnect(),
                ((UIViewModel)context.CurrentViewModel).TradeHubDisconnect()
            });
        }
    }
}