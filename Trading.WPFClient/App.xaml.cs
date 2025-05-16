using System.Configuration;
using System.Data;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Threading;
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
            MainWindow.WindowState = WindowState.Normal;
            //IServiceProvider serviceProvider = CreateServiceProvider();
            //Window window = serviceProvider.GetRequiredService<MainWindow>();
            //window.Show();
            base.OnStartup(e);



            DispatcherUnhandledException += ApplicationDispatcherUnhandledException;
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += OnCurrentDomainFirstChanceException;
            TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedTaskException;
        }
        
        private IServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            return services.BuildServiceProvider();
        }

        private void ApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message,
            "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        // Handle UI thread exceptions
        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"[App] Unhandled Dispatcher exception: {e.Exception}");

        }

        // Handle non-UI thread exceptions
        private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"[App] Unhandled AppDomain exception: {e.ExceptionObject}");
        }


        // Handle task scheduler unhandled exceptions
        private static void OnTaskSchedulerUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine($"[App] Unobserved Task exception: {e.Exception}");
        }

        private static void OnCurrentDomainFirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
        {
            Console.WriteLine($"[App] FirstChanceException: {e.Exception}");

        }
    }

}
