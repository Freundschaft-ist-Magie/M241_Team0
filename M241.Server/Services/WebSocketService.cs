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
        public static ConcurrentDictionary<string, WebSocket> Sockets = new();

        public static async Task UpdateSockets(RoomData latestRoomData, ILogger _logger)
        {
            try
            {
                var json = JsonSerializer.Serialize(new List<RoomData>() { latestRoomData });

                var buffer = Encoding.UTF8.GetBytes(json);
                var segment = new ArraySegment<byte>(buffer);

                foreach (var socket in Sockets.Where(s => s.Key == latestRoomData.RoomId.ToString()))
                {
                    if (socket.Value.State == WebSocketState.Open)
                    {
                        await socket.Value.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to update message {e.Message}", e);
            }
        }
    }
}
