using HMS.Application.Interfaces;
using HMS.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }


        [HttpGet("/api/hotels/{hotelId}/rooms")]
        public async Task<IActionResult> GetRoomsByHotel(
            Guid hotelId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] DateTime? availabilityDate)
        {
            var rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId, minPrice, maxPrice, availabilityDate);
            return Ok(rooms);
        }

        [HttpPost("/api/hotels/{hotelId}/rooms")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddRoom(Guid hotelId, [FromBody] Room room)
        {
            try
            {
                await _roomService.AddRoomToHotelAsync(hotelId, room);
                return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
            }
            catch (ArgumentException ex) { return BadRequest(new { Message = ex.Message }); }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var room = await _roomService.GetRoomByIdAsync(id);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Room room)
        {
            if (id != room.Id) return BadRequest("ID mismatch");
            try
            {
                await _roomService.UpdateRoomAsync(room);
                return NoContent();
            }
            catch (ArgumentException ex) { return BadRequest(new { Message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _roomService.DeleteRoomAsync(id);
            if (!success) return BadRequest(new { Message = "Cannot delete room: Active or future reservations exist" });
            return NoContent();
        }
    }
}