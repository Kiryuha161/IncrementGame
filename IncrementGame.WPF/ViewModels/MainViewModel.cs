using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Incremental.Core.DTOs.Common;
using IncrementGame.WPF.Services;

namespace IncrementGame.WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly GameService _gameService;
        private long _totalPoints;
        private long _clickPower;
        private string _syncStatus = "synced";
        private string _errorMessage = "";
        private string _signalRStatus = "Отключено";
        private int _activeClients = 1;
        private bool _isConnected;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel()
        {
            _gameService = new GameService();

            _gameService.StateChanged += (s, state) =>
            {
                TotalPoints = state.Value;
                ClickPower = state.ClickPower;
            };

            _gameService.SyncStatusChanged += (s, status) =>
            {
                SyncStatus = status;
            };

            _gameService.ErrorOccurred += (s, error) =>
            {
                ErrorMessage = error;
            };

            _gameService.ConnectionStatusChanged += (s, connected) =>
            {
                IsConnected = connected; // 👈 ТЕПЕРЬ ЭТО РАБОТАЕТ
                if (!connected)
                {
                    SyncStatus = "error";
                }
                SignalRStatus = connected ? "Подключено" : "Отключено";
            };

            _gameService.ClientCountChanged += (s, count) =>
            {
                ActiveClients = count;
            };

            Task.Run(async () => await _gameService.LoadStateAsync());
        }

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public string SignalRStatus
        {
            get => _signalRStatus;
            set { _signalRStatus = value; OnPropertyChanged(); }
        }

        public int ActiveClients
        {
            get => _activeClients;
            set { _activeClients = value; OnPropertyChanged(); }
        }

        public long TotalPoints
        {
            get => _totalPoints;
            set { _totalPoints = value; OnPropertyChanged(); }
        }

        public long ClickPower
        {
            get => _clickPower;
            set { _clickPower = value; OnPropertyChanged(); }
        }

        public string SyncStatus
        {
            get => _syncStatus;
            set { _syncStatus = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public void Click()
        {
            _gameService.Click();
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}