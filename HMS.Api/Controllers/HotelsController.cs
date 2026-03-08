using HMS.Application.Interfaces;
using HMS.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HMS.Application.DTO.Auth;

namespace HMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHotels([FromQuery] string? country, [FromQuery] string? city, [FromQuery] int? rating)
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Hotel hotel)
        {
            await _hotelService.CreateHotelAsync(hotel);
            return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] HotelUpdateDto dto)
        {
            try
            {
                var hotelToUpdate = new Hotel
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    Rating = dto.Rating,
                    City = dto.City,
                    Country = dto.Country
                };
                await _hotelService.UpdateHotelAsync(id, hotelToUpdate);
                return NoContent();
            }
            catch (ArgumentException ex) { return BadRequest(new { Message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var success = await _hotelService.DeleteHotelAsync(id);
            if (!success) return BadRequest(new { Message = "Cannot delete hotel: Active rooms or reservations exist" });
            return NoContent();
        }
    }
}