﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using M241.Server.Data;
using M241.Server.Data.Models;
using M241.Server.Common.Dtos;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

namespace M241.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomDatasController : ControllerBase
    {
        private readonly AeroSenseDbContext _context;

        public RoomDatasController(AeroSenseDbContext context)
        {
            _context = context;
        }

        // GET: api/RoomDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomData>>> GetRoomData()
        {
            return await _context.RoomData.ToListAsync();
        }

        // GET: api/RoomDatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomData>> GetRoomData(int id)
        {
            var roomData = await _context.RoomData.FindAsync(id);

            if (roomData == null)
            {
                return NotFound();
            }

            return roomData;
        }

        // PUT: api/RoomDatas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomData(int id, CreateRoomDataDto roomData)
        {
            if (id != roomData.Id)
            {
                return BadRequest();
            }
            var room = await _context.Rooms.FirstAsync(r => r.MACAddress == roomData.MACAddress);

            _context.Entry(roomData.MapToRoomData(room)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomDataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RoomDatas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RoomData>> PostRoomData(CreateRoomDataDto roomData)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(c => c.MACAddress == roomData.MACAddress);
            if(room is null)
            {
                room = new Room()
                {
                    MACAddress = roomData.MACAddress,
                };
            }
            _context.RoomData.Add(roomData.MapToRoomData(room));
            await _context.SaveChangesAsync();
            await UpdateSockets();

            return CreatedAtAction("GetRoomData", new { id = roomData.Id }, roomData);
        }

        // DELETE: api/RoomDatas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomData(int id)
        {
            var roomData = await _context.RoomData.FindAsync(id);
            if (roomData == null)
            {
                return NotFound();
            }

            _context.RoomData.Remove(roomData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomDataExists(int id)
        {
            return _context.RoomData.Any(e => e.Id == id);
        }

        private async Task UpdateSockets()
        {
            var roomDataList = await _context.RoomData.ToListAsync();
            var json = JsonSerializer.Serialize(roomDataList);

            var buffer = Encoding.UTF8.GetBytes(json);
            var segment = new ArraySegment<byte>(buffer);

            foreach (var socket in WebSocketConnectionManager.Sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
