using computerapi.Context;
using computerapi.DTO;
using computerapi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace computerapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var room = await context.Rooms.Include(c => c.User).ToListAsync();
            if (room is null) return NotFound("No computer found!");

            var result = room.Select(c =>
            new {
                Name = c.Name,
                Capacity = c.Capacity,
                UserId = c.UserId
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await context.Rooms.Include(c => c.User).FirstOrDefaultAsync(x => x.Id == id);
            if (room is null) return NotFound("No computer found!");

            var result = new
            {
                Name = room.Name,
                Capacity = room.Capacity,
                UserId = room.UserId
            };

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] RoomDto roomDto)
        {
            if (roomDto is null) return NotFound("Input field empty");
            var room = new Room
            {
                Name = roomDto.Name,
                Capacity = roomDto.Capacity,
                UserId = roomDto.UserId
            };

            await context.Rooms.AddAsync(room);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] RoomDto roomDto)
        {
            var roomToUpdate = await context.Rooms.FirstOrDefaultAsync(c => c.Id == id);
            if (roomToUpdate == null) return NotFound("Computer not found!");

            roomToUpdate.Name = roomDto.Name;
            roomToUpdate.Capacity = roomDto.Capacity;
            roomToUpdate.UserId = roomDto.UserId;

            await context.SaveChangesAsync();
            return Ok("Update successfull!");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var roomToDelete = await context.Rooms.FirstOrDefaultAsync(c => c.Id == id);
            if (roomToDelete == null) return BadRequest("Computer not found!");

            context.Rooms.Remove(roomToDelete);
            await context.SaveChangesAsync();

            return Ok("Removed Successfully");
        }
    }
}
