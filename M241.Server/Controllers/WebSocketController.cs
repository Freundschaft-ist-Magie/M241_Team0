﻿using M241.Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace M241.Server.Controllers
{
    public class WebSocketController : ControllerBase
    {
        private readonly AeroSenseDbContext _context;

        public WebSocketController(AeroSenseDbContext context)
        {
            _context = context;
        }

        private async Task SendRoomData(WebSocket webSocket)
        {
            var roomDataList = await _context.RoomData.ToListAsync();

            var json = JsonSerializer.Serialize(roomDataList, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            var buffer = Encoding.UTF8.GetBytes(json);
            var segment = new ArraySegment<byte>(buffer);

            await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }


        [Route("/api/RoomDatas/ws")]
        public async Task GetWebsocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                WebSocketConnectionManager.Sockets.Add(webSocket);
                await SendRoomData(webSocket);
                // Keep listening to keep connection alive
                var buffer = new byte[1024 * 4];
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                    }
                }

                // Remove on disconnect (optional but cleaner)
                WebSocketConnectionManager.Sockets.TryTake(out var _);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

    }

    public static class WebSocketConnectionManager
    {
        public static ConcurrentBag<WebSocket> Sockets = new();
    }
}
