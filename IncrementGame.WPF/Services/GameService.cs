using Timers = System.Timers;
using Microsoft.AspNetCore.SignalR.Client;
using Incremental.Core.DTOs.Common;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace IncrementGame.WPF.Services
{
    public class GameService : IAsyncDisposable
    {
        private readonly ApiClient _apiClient;
        private HubConnection? _hubConnection;
        private GameStateDto? _currentState;
        private int _pendingClicks;
        private readonly Timers.Timer _syncTimer;
        private bool _isConnected;
        private bool _isReconnecting;
        private string? _myConnectionId; // Добавлено для игнорирования своих сообщений

        public event EventHandler<GameStateDto>? StateChanged;
        public event EventHandler<string>? ErrorOccurred;
        public event EventHandler<string>? SyncStatusChanged;
        public event EventHandler<bool>? ConnectionStatusChanged;
        public event EventHandler<int> ClientCountChanged;

        public GameStateDto? CurrentState => _currentState;
        public bool IsConnected => _isConnected;

        public GameService()
        {
            _apiClient = new ApiClient(maxRetries: 3, retryDelayMs: 1000);

            _syncTimer = new Timers.Timer(3000);
            _syncTimer.Elapsed += async (s, e) => await SyncWithServer();
            _syncTimer.AutoReset = true;

            InitializeSignalR();
        }

        private void InitializeSignalR()
        {
            try
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:7261/gameHub")
                    .WithAutomaticReconnect(new[]
                    {
                        TimeSpan.Zero,
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10)
                    })
                    .ConfigureLogging(logging =>
                    {
                        logging.AddConsole();
                        logging.SetMinimumLevel(LogLevel.Debug);
                    })
                    .Build();

                // Обработчик получения обновлений - ИСПРАВЛЕНО
                _hubConnection.On<GameStateDto>("ReceiveGameStateUpdate", (updatedState) =>
                {
                    // ИГНОРИРУЕМ СВОИ СООБЩЕНИЯ
                    //if (_hubConnection?.ConnectionId == _myConnectionId)
                    //{
                    //    Console.WriteLine($"⏭️ Ignoring own message");
                    //    return;
                    //}

                    Console.WriteLine("═══════════════════════════════════════");
                    Console.WriteLine($"🔥🔥🔥 SIGNALR RECEIVED AT {DateTime.Now:mm:ss.fff}");
                    Console.WriteLine($"🔥 Value: {updatedState.Value}, Power: {updatedState.ClickPower}");
                    Console.WriteLine($"🔥 Thread: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
                    Console.WriteLine("═══════════════════════════════════════");

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Console.WriteLine($"📢 DISPATCHER INVOKE on thread {System.Threading.Thread.CurrentThread.ManagedThreadId}");

                        if (_currentState != null)
                        {
                            Console.WriteLine($"📢 Current state BEFORE: {_currentState.Value}");

                            _currentState.Value = updatedState.Value;
                            _currentState.ClickPower = updatedState.ClickPower;
                            _pendingClicks = 0; // 👈 Сбрасываем pending после получения подтвержденного состояния

                            Console.WriteLine($"📢 Current state AFTER: {_currentState.Value}");

                            StateChanged?.Invoke(this, _currentState);
                            Console.WriteLine($"📢 StateChanged event fired");
                        }
                        else
                        {
                            Console.WriteLine($"❌ ERROR: _currentState is null!");
                        }
                    });
                });

                _hubConnection.On<int>("UpdateClientCount", (count) =>
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Console.WriteLine($"👥 Активных клиентов: {count}");
                        ClientCountChanged?.Invoke(this, count); 
                    });
                });

                // Обработчики событий подключения
                _hubConnection.Closed += async (error) =>
                {
                    Debug.WriteLine($"Connection closed: {error?.Message}");
                    _isConnected = false;
                    ConnectionStatusChanged?.Invoke(this, false);
                    ErrorOccurred?.Invoke(this, "Соединение потеряно");
                    await Task.Delay(5000);
                    await ConnectSignalRAsync();
                };

                _hubConnection.Reconnected += (connectionId) =>
                {
                    Console.WriteLine($"Reconnected: {connectionId}");
                    _isConnected = true;
                    _isReconnecting = false;
                    _myConnectionId = connectionId; // 👈 Обновляем ID при переподключении
                    ConnectionStatusChanged?.Invoke(this, true);
                    ErrorOccurred?.Invoke(this, "Соединение восстановлено");
                    return Task.CompletedTask;
                };

                _hubConnection.Reconnecting += (error) =>
                {
                    Console.WriteLine($"Reconnecting: {error?.Message}");
                    _isReconnecting = true;
                    ErrorOccurred?.Invoke(this, "Переподключение...");
                    return Task.CompletedTask;
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR init error: {ex.Message}");
                ErrorOccurred?.Invoke(this, $"SignalR error: {ex.Message}");
            }
        }

        public async Task ConnectSignalRAsync()
        {
            try
            {
                Console.WriteLine("Connecting to SignalR...");
                await _hubConnection.StartAsync();
                _isConnected = true;
                _isReconnecting = false;
                _myConnectionId = _hubConnection?.ConnectionId; // 👈 ЗАПОМИНАЕМ ID
                ConnectionStatusChanged?.Invoke(this, true);
                ErrorOccurred?.Invoke(this, "Подключено к реальному времени");
                Console.WriteLine($"SignalR connected. Connection ID: {_hubConnection?.ConnectionId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR connection error: {ex.Message}");
                _isConnected = false;
                ConnectionStatusChanged?.Invoke(this, false);
                ErrorOccurred?.Invoke(this, $"Ошибка подключения SignalR: {ex.Message}");
            }
        }

        public async void Click()
        {
            if (_currentState == null) return;

            var oldValue = _currentState.Value;
            _currentState.Value += _currentState.ClickPower;
            _pendingClicks++;

            Console.WriteLine($"👆 CLICK: {oldValue} -> {_currentState.Value}");
            Console.WriteLine($"👆 StateChanged firing from Click");

            StateChanged?.Invoke(this, _currentState);
            SyncStatusChanged?.Invoke(this, "syncing");

            if (!_syncTimer.Enabled)
            {
                _syncTimer.Start();
            }
        }

        public async Task LoadStateAsync()
        {
            try
            {
                var response = await _apiClient.GetAsync<GameStateDto>("/points");

                if (response.Success && response.Data != null)
                {
                    _currentState = response.Data;
                    _pendingClicks = 0;

                    Console.WriteLine($"📊 LOAD STATE: Value={_currentState.Value}");
                    Console.WriteLine($"📊 StateChanged firing from LoadState");

                    StateChanged?.Invoke(this, _currentState);
                    SyncStatusChanged?.Invoke(this, "synced");

                    // Подключаем SignalR
                    if (_hubConnection?.State == HubConnectionState.Disconnected)
                    {
                        await ConnectSignalRAsync();
                    }
                }
                else
                {
                    ErrorOccurred?.Invoke(this, response.Message ?? "Не удалось загрузить состояние");
                    SyncStatusChanged?.Invoke(this, "error");
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke(this, ex.Message);
                SyncStatusChanged?.Invoke(this, "error");
            }
        }

        private async Task SyncWithServer()
        {
            if (_pendingClicks == 0 || _currentState == null) return;

            try
            {
                var response = await _apiClient.PostAsync<object>("/points/state", _currentState);

                if (response.Success)
                {
                    Console.WriteLine($"💾 State saved to server: {_currentState.Value}");

                    int savedClicks = _pendingClicks;
                    _pendingClicks = 0;
                    SyncStatusChanged?.Invoke(this, "synced");

                    // 👇 Отправляем SignalR ТОЛЬКО после успешного сохранения
                    if (_hubConnection?.State == HubConnectionState.Connected)
                    {
                        Console.WriteLine($"📤 Broadcasting saved state: {_currentState.Value} (from {savedClicks} clicks)");
                        await _hubConnection.SendAsync("SendGameStateUpdate", _currentState);
                    }

                    if (_pendingClicks == 0)
                    {
                        _syncTimer.Stop();
                    }
                }
                else
                {
                    SyncStatusChanged?.Invoke(this, "error");
                    ErrorOccurred?.Invoke(this, response.Message ?? "Ошибка синхронизации");
                }
            }
            catch (Exception ex)
            {
                SyncStatusChanged?.Invoke(this, "error");
                ErrorOccurred?.Invoke(this, ex.Message);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.DisposeAsync();
            }
            _syncTimer?.Dispose();
        }
    }
}