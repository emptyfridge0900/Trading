using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Trading.WPFClient.ViewModels;

namespace Trading.WPFClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(MainWindow)
            };
            MainWindow.Show();
            //IServiceProvider serviceProvider = CreateServiceProvider();
            //Window window = serviceProvider.GetRequiredService<MainWindow>();
            //window.Show();
            base.OnStartup(e);
        }
        private IServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            return services.BuildServiceProvider();
        }
    }

}
