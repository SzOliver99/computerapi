using computerapi.Context;
using computerapi.DTO;
using computerapi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using static System.Reflection.Metadata.BlobBuilder;

namespace computerapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComputerController(ApplicationDbContext context) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var computer = await context.Computers.Include(c => c.Room).ToListAsync();
            if (computer is null) return NotFound("No computer found!");

            var result = computer.Select(c =>
            new {
                Model = c.Model,
                ManufacturedDate = c.ManufacturedDate,
                Room = c.Room,
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var computer = await context.Computers.Include(c => c.Room).FirstOrDefaultAsync(x => x.Id == id);
            if (computer is null) return NotFound("No computer found!");

            var result = new
            {
                Model = computer.Model,
                ManufacturedDate = computer.ManufacturedDate,
                Room = computer.Room,
            };

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] ComputerDto computerDto)
        {
            if (computerDto is null) return NotFound("Input field empty");
            var computer = new Computer
            {
                Model = computerDto.Model,
                ManufacturedDate = computerDto.ManufacturedDate,
                RoomId = computerDto.RoomId,
            };

            await context.Computers.AddAsync(computer);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = computer.Id}, computer);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] ComputerDto computerDto)
        {
            var computerToUpdate = await context.Computers.FirstOrDefaultAsync(c => c.Id == id);
            if (computerToUpdate == null) return NotFound("Computer not found!");

            computerToUpdate.Model = computerDto.Model;
            computerToUpdate.ManufacturedDate = computerDto.ManufacturedDate;
            computerToUpdate.RoomId = computerDto.RoomId;

            await context.SaveChangesAsync();
            return Ok("Update successfull!");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var bookToDelete = await context.Computers.FirstOrDefaultAsync(c => c.Id == id);
            if (bookToDelete == null) return BadRequest("Computer not found!");

            context.Computers.Remove(bookToDelete);
            await context.SaveChangesAsync();

            return Ok("Removed Successfully");
        }
    }
}
