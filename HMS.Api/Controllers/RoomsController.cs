using AutoMapper;
using HMS.Application.Common;
using HMS.Application.DTO.Auth;
using HMS.Application.Interfaces;
using HMS.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Api.Controllers
{
    [Route("api/hotels/{hotelId}/rooms")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public RoomsController(IRoomService roomService, IMapper mapper)
        {
            _roomService = roomService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomsByHotel(
            Guid hotelId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] DateTime? availabilityDate)
        {
            var rooms = await _roomService.GetRoomsByHotelIdAsync(hotelId, minPrice, maxPrice, availabilityDate);
            var dtos = _mapper.Map<IEnumerable<RoomResponseDto>>(rooms);
            return Ok(ApiResponse<IEnumerable<RoomResponseDto>>.Ok(dtos));
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetById(Guid hotelId, Guid roomId)
        {
            var room = await _roomService.GetRoomByIdAsync(roomId);
            if (room == null || room.HotelId != hotelId)
                return NotFound(ApiResponse<RoomResponseDto>.Fail("Room not found in this hotel"));

            var dto = _mapper.Map<RoomResponseDto>(room);
            return Ok(ApiResponse<RoomResponseDto>.Ok(dto));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddRoom(Guid hotelId, [FromBody] RoomCreateDto dto)
        {
            var room = _mapper.Map<Room>(dto);
            await _roomService.AddRoomToHotelAsync(hotelId, room);
            var response = _mapper.Map<RoomResponseDto>(room);
            return CreatedAtAction(nameof(GetById), new { hotelId, roomId = room.Id },
                ApiResponse<RoomResponseDto>.Ok(response, "Room added successfully"));
        }

        [HttpPut("{roomId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(Guid hotelId, Guid roomId, [FromBody] RoomUpdateDto dto)
        {
            var room = _mapper.Map<Room>(dto);
            room.Id = roomId;
            room.HotelId = hotelId;
            await _roomService.UpdateRoomAsync(room);
            return NoContent();
        }

        [HttpDelete("{roomId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(Guid hotelId, Guid roomId)
        {
            var success = await _roomService.DeleteRoomAsync(roomId);
            if (!success)
                return BadRequest(ApiResponse<object>.Fail("Cannot delete room: it has active or future reservations"));

            return NoContent();
        }
    }
}