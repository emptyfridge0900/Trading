using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Trading.Common.Models;

namespace Trading.WPFClient.ViewModels
{
    public class HistoryViewModel : ViewModelBase
    {
        private readonly HubConnection _hubConnection;
        private ObservableCollection<TradeRecord> _records;
        public ObservableCollection<TradeRecord> Records 
        { 
            get { return _records; } 
            set
            {
                _records = value;
                OnPropertyChanged();
            }
        }

        private string _jwt;
        public HistoryViewModel(string jwt)
        {
            _records = new ObservableCollection<TradeRecord>();
            _jwt = jwt;
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"http://localhost:5053/trading?access_token={_jwt}")
                //, options =>
                //{
                //    //options.SkipNegotiation = true;
                //    //options.Transports = HttpTransportType.WebSockets;
                //    options.AccessTokenProvider = () => Task.FromResult(_jwt);
                //})
                .WithServerTimeout(TimeSpan.FromSeconds(30))// 30 sec by default
                .WithKeepAliveInterval(TimeSpan.FromSeconds(15))// 15 sec by default
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On("ReceiveRecords", (List<TradeRecord> records) =>
            {
                Records = new ObservableCollection<TradeRecord>(records);
            });

            _hubConnection.Reconnecting += (ex) =>
            {
                Console.WriteLine("Reconnectiong...");
                Console.WriteLine(_hubConnection.State);

                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (f) =>
            {
                Console.WriteLine("Successfully reconnected!");
                Console.WriteLine(_hubConnection.State);
                //await _hubConnection.InvokeAsync("JoinGroup", TickerName);
                return Task.CompletedTask;
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
        }
        public async void Connect()
        {
            try
            {
                await ConnectWithRetryAsync(_hubConnection);
                await _hubConnection.InvokeAsync("SendTradingHistory");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR connection failed: {ex.Message}");
            }
        }

        //WithAutomaticReconnect() won't configure the HubConnection to retry initial start failures,
        //so start failures need to be handled manually
        public static async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token = default)
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
                catch(Exception ex) 
                {
                    // Failed to connect, trying again in 5000 ms.
                    Debug.Assert(connection.State == HubConnectionState.Disconnected);
                    Console.WriteLine("Not connected, try again in 5 seconds");
                    Console.WriteLine(ex);
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
    }
}
