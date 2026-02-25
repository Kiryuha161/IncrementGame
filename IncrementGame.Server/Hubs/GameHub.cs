using Microsoft.AspNetCore.SignalR;
using Incremental.Core.DTOs.Common;
using Serilog;
using System.Collections.Concurrent;

namespace IncrementGame.Server.Hubs
{
    public class GameHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _connectedClients = new();

        public async Task SendGameStateUpdate(GameStateDto gameState)
        {
            try
            {
                await Clients.All.SendAsync("ReceiveGameStateUpdate", gameState);
                Log.Information($"✅ Сообщение отправлено всем {_connectedClients.Count} клиентам");
            }
            catch (Exception ex)
            {
                Log.Error($"❌ Ошибка отправки: {ex.Message}");
            }

            // Отправляем всем обновленный счетчик клиентов
            await Clients.All.SendAsync("UpdateClientCount", _connectedClients.Count);
        }

        public override async Task OnConnectedAsync()
        {
            _connectedClients.TryAdd(Context.ConnectionId, Context.ConnectionId);
            Log.Information($"✅ Клиент подключен: {Context.ConnectionId}. Всего: {_connectedClients.Count}");

            // Отправляем всем новый счетчик
            await Clients.All.SendAsync("UpdateClientCount", _connectedClients.Count);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connectedClients.TryRemove(Context.ConnectionId, out _);
            Log.Information($"❌ Клиент отключен: {Context.ConnectionId}. Всего: {_connectedClients.Count}");

            // Отправляем всем новый счетчик
            await Clients.All.SendAsync("UpdateClientCount", _connectedClients.Count);

            await base.OnDisconnectedAsync(exception);
        }
    }
}