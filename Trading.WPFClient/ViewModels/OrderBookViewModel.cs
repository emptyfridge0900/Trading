using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Trading.Common.Models;
using Trading.WPFClient.Models;

namespace Trading.WPFClient.ViewModels
{
    public class OrderBookViewModel: ViewModelBase
    {
        private readonly HubConnection _hubConnection;
        private string _tickerName;
        public string TickerName
        {
            get { return _tickerName; }
            set { 
                _hubConnection.InvokeAsync("UnjoinGroup", _tickerName);
                _tickerName = value;
                OnPropertyChanged(nameof(TickerName));
                _hubConnection.InvokeAsync("JoinGroup", _tickerName);
            }
        }

        private ObservableCollection<Order> _orders;
        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders));
            }
        }

        private ObservableCollection<string> _tickers;
        public ObservableCollection<string> Tickers
        {
            get { return _tickers; }
            set
            {
                _tickers = value;
                OnPropertyChanged(nameof(Tickers));
            }
        }

        public OrderBookViewModel(string symbol, List<Ticker> tickers)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5053/ticker")
                .WithServerTimeout(TimeSpan.FromSeconds(30))// 30 sec by default
                .WithKeepAliveInterval(TimeSpan.FromSeconds(15))// 15 sec by default
                .WithAutomaticReconnect()
                .Build();
            
            _hubConnection.On("ReceiveOrderBook", (List<Order> x) => {
                try
                {
                    Orders = new ObservableCollection<Order>(x);
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
            _hubConnection.Reconnected += async (f) =>
            {
                Console.WriteLine("Successfully reconnected!");
                Console.WriteLine(_hubConnection.State);
                await _hubConnection.InvokeAsync("JoinGroup", TickerName);
            };
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
            _tickerName = symbol;
            TickerName = symbol;
            _tickers = new ObservableCollection<string>();
            Tickers = new ObservableCollection<string>(tickers.Select(x=>x.Symbol));
            _orders = new ObservableCollection<Order>();
        }

        public async void Connect()
        {
            try
            {
                await ConnectWithRetryAsync(_hubConnection);
                await _hubConnection.InvokeAsync("JoinGroup", TickerName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR connection failed: {ex.Message}");
            }

        }
        public static async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token = default)
        {
            while (true)
            {
                try
                {
                    await connection.StartAsync(token);
                    Debug.Assert(connection.State == HubConnectionState.Connected);
                    Console.WriteLine("Connected to the server!");
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
        public async void Disconnect()
        {
            if (_hubConnection.State != HubConnectionState.Disconnected)
            {
                await _hubConnection.InvokeAsync("UnjoinGroup", TickerName);
                Console.WriteLine("Remove from group");
                await _hubConnection.StopAsync();
                Console.WriteLine("Stop request sent");
            }

        }

    }
}
