using HMS.Application.Interfaces;
using HMS.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HMS.Application.DTO.Auth;
using AutoMapper;
using HMS.Application.Mappings;
using HMS.Application.Common;

namespace HMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly IMapper _mapper;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()));
        }

        [HttpGet]
        public async Task<IActionResult> GetHotels([FromQuery] string? country, [FromQuery] string? city, [FromQuery] int? rating)
        {
            var hotels = await _hotelService.GetFilteredHotelsAsync(country, city, rating);
            var dtos = _mapper.Map<IEnumerable<HotelResponseDto>>(hotels);
            return Ok(ApiResponse<IEnumerable<HotelResponseDto>>.Ok(dtos));
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
        public async Task<IActionResult> Create([FromBody] HotelCreateDto dto)
        {
            var hotel = _mapper.Map<Hotel>(dto);
            await _hotelService.CreateHotelAsync(hotel);
            var response = _mapper.Map<HotelResponseDto>(hotel);
            return CreatedAtAction(nameof(GetById), new { id = hotel.Id },
                ApiResponse<HotelResponseDto>.Ok(response, "Hotel created successfully"));
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