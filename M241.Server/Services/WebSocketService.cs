using M241.Server.Controllers;
using M241.Server.Data;
using M241.Server.Data.Models;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace M241.Server.Services
{
    public static class WebSocketService
    {
        private static bool isUpdatingSockets;
        public static ConcurrentBag<WebSocket> Sockets = new();

        public static async Task UpdateSockets(RoomData latestRoomData, ILogger _logger)
        {
            if (isUpdatingSockets) return;
            isUpdatingSockets = true;
            try
            {
                var json = JsonSerializer.Serialize(new List<RoomData>() { latestRoomData });

                var buffer = Encoding.UTF8.GetBytes(json);
                var segment = new ArraySegment<byte>(buffer);

                foreach (var socket in Sockets)
                {
                    if (socket.State == WebSocketState.Open)
                    {
                        await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update message {e.Message}", e);
            }
            finally
            {
                isUpdatingSockets = false;
            }
        }
    }
}
