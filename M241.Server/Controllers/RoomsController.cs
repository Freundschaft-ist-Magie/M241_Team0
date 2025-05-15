using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using M241.Server.Data;
using M241.Server.Data.Models;
using M241.Server.Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using M241.Server.Services;

namespace M241.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly AeroSenseDbContext _context;
        private readonly MqttService _mqttService;

        public RoomsController(AeroSenseDbContext context, MqttService mqttService)
        {
            _context = context;
            _mqttService = mqttService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            return await _context.Rooms.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }

        [HttpGet("ping/{macAddress}")]
        public async Task<ActionResult<Room>> GetRoom(string macAddress)
        {
            await _mqttService.PingRoom(macAddress);
            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Roles= "Administrator")]
        public async Task<IActionResult> PutRoom(int id, Room roomDto)
        {
            if (id != roomDto.Id)
            {
                return BadRequest();
            }

            _context.Entry(roomDto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
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

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Room>> PostRoom(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoom", new { id = room.Id }, room);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}
