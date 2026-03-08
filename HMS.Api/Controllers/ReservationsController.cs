using Microsoft.AspNetCore.Mvc;
using HMS.Application.Interfaces;
using HMS.Core.Models;
using HMS.Application.DTO.Auth;
using Microsoft.AspNetCore.Authorization;

namespace HMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [Authorize(Roles = "Guest")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservationRequest request)
        {
            try
            {
                var result = await _reservationService.CreateBookingAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var reservations = await _reservationService.GetUserReservationsAsync(userId);
            return Ok(reservations);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _reservationService.DeleteReservationAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPut("{id}/dates")]
        [Authorize(Roles = "Guest,Admin")]
        public async Task<IActionResult> UpdateDates(Guid id, [FromBody] UpdateDatesRequest request)
        {
            try
            {
                var updated = await _reservationService.UpdateReservationDatesAsync(
                    id,
                    request.NewCheckIn,
                    request.NewCheckOut
                );

                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}