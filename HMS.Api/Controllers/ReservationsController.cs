using AutoMapper;
using HMS.Application.Common;
using HMS.Application.DTO.Auth;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IMapper _mapper;

        public ReservationsController(IReservationService reservationService, IMapper mapper)
        {
            _reservationService = reservationService;
            _mapper = mapper;
        }

        [HttpPost("api/hotels/{hotelId}/reservations")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> Create(Guid hotelId, [FromBody] ReservationCreateDto dto)
        {
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var reservation = await _reservationService.CreateBookingAsync(dto, callerId, hotelId);
            var response = _mapper.Map<ReservationResponseDto>(reservation);
            return CreatedAtAction(nameof(GetById), new { id = reservation.Id },
                ApiResponse<ReservationResponseDto>.Ok(response, "Reservation created successfully."));
        }

        [HttpGet("api/reservations/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null)
                return NotFound(ApiResponse<ReservationResponseDto>.Fail("Reservation not found."));

            var dto = _mapper.Map<ReservationResponseDto>(reservation);
            return Ok(ApiResponse<ReservationResponseDto>.Ok(dto));
        }

        [HttpGet("api/reservations")]
        public async Task<IActionResult> Search(
            [FromQuery] string? guestId,
            [FromQuery] Guid? hotelId,
            [FromQuery] Guid? roomId,
            [FromQuery] DateTime? date,
            [FromQuery] bool? activeOnly)
        {
            var reservations = await _reservationService.SearchAsync(guestId, hotelId, roomId, date, activeOnly);
            var dtos = _mapper.Map<IEnumerable<ReservationResponseDto>>(reservations);
            return Ok(ApiResponse<IEnumerable<ReservationResponseDto>>.Ok(dtos));
        }

        [HttpPut("api/reservations/{id}/dates")]
        [Authorize(Roles = "Guest,Admin")]
        public async Task<IActionResult> UpdateDates(Guid id, [FromBody] UpdateDatesRequest request)
        {
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = User.IsInRole("Admin");

            var updated = await _reservationService.UpdateReservationDatesAsync(
                id, request.NewCheckIn, request.NewCheckOut, callerId, isAdmin);
            var dto = _mapper.Map<ReservationResponseDto>(updated);
            return Ok(ApiResponse<ReservationResponseDto>.Ok(dto, "Dates updated successfully."));
        }

        [HttpDelete("api/reservations/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = User.IsInRole("Admin");

            var success = await _reservationService.DeleteReservationAsync(id, callerId, isAdmin);
            if (!success)
                return NotFound(ApiResponse<object>.Fail("Reservation not found."));

            return NoContent();
        }
    }
}