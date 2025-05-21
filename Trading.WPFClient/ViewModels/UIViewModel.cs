using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR.Client;
using Trading.WPFClient.Commands;
using Trading.Common.Models;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using Trading.WPFClient.Utility;


namespace Trading.WPFClient.ViewModels
{
    public class UIViewModel:ViewModelBase
    {
        private readonly HubConnection _hubConnection;
        private readonly HubConnection _tradeHubConnection;
        private ObservableCollection<Ticker> _tickers;
        
        public ObservableCollection<Ticker> Tickers 
        { 
            get { return _tickers; }
            set
            {
                _tickers = value;
                OnPropertyChanged();
            } 
        }

        private Ticker _ticker;
        public Ticker Ticker
        {
            get { return _ticker; }
            set { 
                _ticker = value;
                Symbol = value.Symbol;
                OnPropertyChanged();
                IsOrderBookAvailable = true;
            }
        }
        static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AllocConsole();
        }

        public string Jwt;
        public string Name;

        private string _selectedSide = "Bid";
        public string SelectedSide 
        {
            get => _selectedSide;
            set
            {
                if (_selectedSide != value)
                {
                    _selectedSide = value;
                    OnPropertyChanged();
                }
            }
        }


        public List<string> Sides { get; } = new List<string> { "Bid", "Ask" };
        private string _symbol;
        public string Symbol 
        {
            get => _symbol;
            set
            {
                _symbol = value;
                OnPropertyChanged();
            }
        }
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; } = 10;



        private const int PageSize = 10;
        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        private bool _isOrderBookAvailable;
        public bool IsOrderBookAvailable
        {
            get=> _isOrderBookAvailable;
            set
            {
                _isOrderBookAvailable = value;
                OnPropertyChanged();
            }
        }
        public UIViewModel()
        {
            _tickers = new ObservableCollection<Ticker>();
            _ticker = new Ticker();
            _symbol = "TSLA";
            Name = JwtGen.GenerateName();
            Jwt = JwtGen.GenerateJwtToken(Name);
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5053/ticker")
                .WithServerTimeout(TimeSpan.FromSeconds(30))// 30 sec by default
                .WithKeepAliveInterval(TimeSpan.FromSeconds(15))// 15 sec by default
                .WithAutomaticReconnect()
                .Build();

            _tradeHubConnection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:5053/trading?access_token={Jwt}")
                .WithServerTimeout(TimeSpan.FromSeconds(30))// 30 sec by default
                .WithKeepAliveInterval(TimeSpan.FromSeconds(15))// 15 sec by default
                .WithAutomaticReconnect()
                .Build();
            

            NativeMethods.AllocConsole();

            _hubConnection.On("ReceiveTickerList", (PaginatedResult<Ticker> x) => {
                try
                {
                    Tickers = new ObservableCollection<Ticker>(x.Results);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                
            });


            _hubConnection.Reconnecting += (ex) =>
            {
                Console.WriteLine("Reconnectiong...");
                Console.WriteLine(_hubConnection.State);
        
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += async (msg) =>
            {
                Console.WriteLine("Successfully reconnected!");
                Console.WriteLine(_hubConnection.State);
                await _hubConnection.InvokeAsync("SendTickerList", CurrentPage, PageSize);
                await _hubConnection.InvokeAsync("Login", Name);
            };

            //try reconnect 4 time
            _hubConnection.Closed += async (ex) =>
            {
                Console.WriteLine("Closed event");
                if (ex == null)
                {
                    Console.WriteLine("Connection closed without error.");
                }
                else
                {
                    Console.WriteLine($"Connection closed due to an error: {ex}");

                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await ConnectWithRetryAsync(_hubConnection);
                }
            };


            OpenHistory = new OpenHistoryCommand(this);
            OpenOrder = new OpenOrderBookCommand(this);
            CloseWindow = new DefaultCommand(()=>Exit(),null);
            SubmitOrderCommand = new DefaultCommand(() => SubmitOrder(), null);

        }
        public async Task Connect()
        {
            try
            {
                //await _hubConnection.StartAsync();
                await ConnectWithRetryAsync(_hubConnection);
                await _hubConnection.InvokeAsync("SendTickerList", CurrentPage, PageSize);
                await _hubConnection.InvokeAsync("Login", Name);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR connection failed: {ex.Message}");
            }
        }
        public async Task TradeHubConnect()
        {
            try
            {
                await ConnectWithRetryAsync(_tradeHubConnection);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR connection failed: {ex.Message}");
            }
        }

        //WithAutomaticReconnect() won't configure the HubConnection to retry initial start failures,
        //so start failures need to be handled manually
        public static async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token=default)
        {
            while (true)
            {
                try
                {
                    await connection.StartAsync(token);
                    Debug.Assert(connection.State == HubConnectionState.Connected);
                    Console.WriteLine("Connected to the SignalR server!");
                    return true;
                }
                catch when (token.IsCancellationRequested)
                {
                    return false;
                }
                catch
                {
                    // Failed to connect, trying again in 5000 ms.
                    Debug.Assert(connection.State == HubConnectionState.Disconnected);
                    Console.WriteLine("Not connected, try again in 5 seconds");
                    await Task.Delay(5000);
                }
            }
        }
        public async Task Disconnect()
        {
            if (_hubConnection.State != HubConnectionState.Disconnected)
            {
                await _hubConnection.StopAsync();
                Console.WriteLine("Stop request sent");
            }
        }
        public async Task TradeHubDisconnect()
        {
            if (_tradeHubConnection.State != HubConnectionState.Disconnected)
            {
                await _tradeHubConnection.StopAsync();
                Console.WriteLine("Stop request sent");
            }
        }
        public ICommand OpenHistory { get; set; }
        public ICommand OpenOrder { get; set; }
        public ICommand CloseWindow { get; set; }
        public ICommand SubmitOrderCommand { get; }

        private async Task SubmitOrder()
        {
            var symbols = Tickers.Select(x => x.Symbol);
            if (symbols.Contains(Symbol))
            {
                await _tradeHubConnection.InvokeAsync("Order", SelectedSide, Symbol, Price, Quantity);
                MessageBox.Show("Your order has been successfully completed", "Success", MessageBoxButton.OK, MessageBoxImage.None);
            }
            else
                MessageBox.Show("Please enter a valid ticker symbol", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

        }

        private async Task Exit()
        {
            await Disconnect();
            await TradeHubDisconnect();
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }


    }

}
