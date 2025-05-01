using M241.Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using M241.Server.Common.Dtos;
using AutoMapper;
using Radzen.Blazor.Rendering;
using M241.Server.Services;

namespace M241.Server.Controllers
{
    public class WebSocketController : ControllerBase
    {
        private readonly AeroSenseDbContext _context;
        private readonly IMapper _mapper;

        public WebSocketController(AeroSenseDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        private async Task SendRoomData(WebSocket webSocket)
        {
            var roomDataList = await _context.RoomData.ToListAsync();
            var mappedRoomData = _mapper.Map<List<RoomDataDto>>(roomDataList);

            var json = JsonSerializer.Serialize(mappedRoomData, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            var buffer = Encoding.UTF8.GetBytes(json);
            var segment = new ArraySegment<byte>(buffer);

            await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task SendRoomData(WebSocket webSocket, int id)
        {
            var roomData = await _context.RoomData.FindAsync(id);
            var mappedRoomData = _mapper.Map<RoomDataDto>(roomData);

            var json = JsonSerializer.Serialize(mappedRoomData, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            var buffer = Encoding.UTF8.GetBytes(json);
            var segment = new ArraySegment<byte>(buffer);

            await webSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }


        [Route("/api/RoomDatas/ws")]
        public async Task GetRooms()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                WebSocketService.Sockets.Add(webSocket);
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
                WebSocketService.Sockets.TryTake(out var _);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        [Route("/api/RoomDatas/ws/{id}")]
        public async Task GetRoomById(int id)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                WebSocketService.Sockets.Add(webSocket);
                await SendRoomData(webSocket, id);
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
                WebSocketService.Sockets.TryTake(out var _);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

    }
}
