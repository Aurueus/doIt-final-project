using HMS.Application.Interfaces;
using HMS.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly IRoomService _roomService;
        public HotelsController(IHotelService hotelService, IRoomService roomService)
        {
            _hotelService = hotelService;
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHotels(
            [FromQuery] string? country,
            [FromQuery] string? city,
            [FromQuery] int? rating)
        {
            var hotels = await _hotelService.GetFilteredHotelsAsync(country, city, rating);
            return Ok(hotels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            if (hotel == null) return NotFound();
            return Ok(hotel);
        }

        [HttpGet("{hotelId}/rooms")]
        public async Task<IActionResult> GetRooms(Guid hotelId, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId, minPrice, maxPrice);
            return Ok(rooms);
        }

        [HttpPost("{hotelId}/rooms")]
        public async Task<IActionResult> AddRoom(Guid hotelId, Room room)
        {
            try
            {
                await _roomService.AddRoomToHotelAsync(hotelId, room);
                return Ok(room);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Hotel hotel)
        {
            await _hotelService.CreateHotelAsync(hotel);
            return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _hotelService.DeleteHotelAsync(id);
            if (!success)
            {
                return BadRequest("Cannot delete hotel: It may have active rooms or reservations.");
            }
            return NoContent();
        }
    }
}