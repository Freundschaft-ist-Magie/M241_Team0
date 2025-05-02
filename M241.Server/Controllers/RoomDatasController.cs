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
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using M241.Server.Services;

namespace M241.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomDatasController : ControllerBase
    {
        private readonly AeroSenseDbContext _context;
        private readonly ILogger<RoomDatasController> _logger;

        public RoomDatasController(AeroSenseDbContext context, ILogger<RoomDatasController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/RoomDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomData>>> GetRoomData()
        {
            return await _context.RoomData.Include(r => r.Room).ToListAsync();
        }

        // GET: api/RoomDatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomData>> GetRoomData(int id)
        {
            var roomData = await _context.RoomData.Include(r => r.Room).FirstOrDefaultAsync(r => r.Id == id);

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
        public async Task<ActionResult<RoomData>> PostRoomData(CreateRoomDataDto createRoomData)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(c => c.MACAddress == createRoomData.MACAddress);
            if(room is null)
            {
                room = new Room()
                {
                    MACAddress = createRoomData.MACAddress,
                };
            }
            var roomData = createRoomData.MapToRoomData(room);
            _context.RoomData.Add(roomData);
            await _context.SaveChangesAsync();
            var newRoom = await _context.RoomData.Include(r => r.Room).FirstOrDefaultAsync(r => r.Id == roomData.Id);
            await WebSocketService.UpdateSockets(newRoom, _logger);

            return CreatedAtAction("GetRoomData", new { id = newRoom.Id }, newRoom);
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
    }
}
