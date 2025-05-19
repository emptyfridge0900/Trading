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
using Trading.WPFClient.Services;
using Trading.WPFClient.Views;
using Microsoft.Extensions.Logging;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace Trading.WPFClient.ViewModels
{
    public class UIViewModel:ViewModelBase
    {
        private readonly HubConnection _hubConnection;
        private readonly ILogger<UIViewModel> _logger;
        private Window _mainWindow;
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
                OnPropertyChanged();
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
        public UIViewModel(Window mainWinow)
        {
            Name = JwtGen.GenerateName();
            Jwt = JwtGen.GenerateJwtToken(Name);
            OrderPageView = new OrderViewModel(Jwt);
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5053/ticker")
                .WithServerTimeout(TimeSpan.FromSeconds(30))// 30 sec by default
                .WithKeepAliveInterval(TimeSpan.FromSeconds(15))// 15 sec by default
                .WithAutomaticReconnect()
                .Build();

            NativeMethods.AllocConsole();

             _hubConnection.On("ReceiveTickerList", (List<Ticker> x) => {
                 try
                 {
                    Tickers = new ObservableCollection<Ticker>(x);
                 }
                 catch (Exception ex)
                 {
                 }
                
            });

            _hubConnection.On("ReceiveJwt", (string jwt) => Jwt = jwt);

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
                await _hubConnection.InvokeAsync("SendTickerLit");
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

            _mainWindow = mainWinow;

            OpenHistory = new OpenHistoryCommand(this,_mainWindow);
            OpenOrder = new OpenOrderBookCommand(this,_mainWindow);
        }
        public async void Connect()
        {
            try
            {
                //await _hubConnection.StartAsync();
                await ConnectWithRetryAsync(_hubConnection);
                await _hubConnection.InvokeAsync("SendTickerLit");
                
                await _hubConnection.InvokeAsync("Login", Name);
                

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
        public async void Disconnect()
        {
            if (_hubConnection.State != HubConnectionState.Disconnected)
            {
                await _hubConnection.StopAsync();
                Console.WriteLine("Stop request sent");
            }
        }
        public ICommand OpenHistory { get; set; }
        public ICommand OpenOrder { get; set; }


    }

}
