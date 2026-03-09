using AutoMapper;
using HMS.Application.Common;
using HMS.Application.DTO.Auth;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.Api.Controllers
{
    [Route("api/guests")]
    [ApiController]
    [Authorize]
    public class GuestsController : ControllerBase
    {
        private readonly IGuestService _guestService;
        private readonly IMapper _mapper;

        public GuestsController(IGuestService guestService, IMapper mapper)
        {
            _guestService = guestService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(string id)
        {
            var guest = await _guestService.GetGuestByIdAsync(id);
            if (guest == null)
                return NotFound(ApiResponse<GuestResponseDto>.Fail("Guest not found."));

            var dto = _mapper.Map<GuestResponseDto>(guest);
            return Ok(ApiResponse<GuestResponseDto>.Ok(dto));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] GuestUpdateDto dto)
        {
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && callerId != id)
                return Forbid();

            var success = await _guestService.UpdateGuestAsync(id, dto);
            if (!success)
                return NotFound(ApiResponse<object>.Fail("Guest not found."));

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _guestService.DeleteGuestAsync(id);
            if (!success)
                return BadRequest(ApiResponse<object>.Fail("Cannot delete guest: they have active or future reservations."));

            return NoContent();
        }
    }
}