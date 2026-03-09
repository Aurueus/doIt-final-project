using AutoMapper;
using HMS.Application.Common;
using HMS.Application.DTO.Auth;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Api.Controllers
{
    [Route("api/hotels/{hotelId}/managers")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ManagersController : ControllerBase
    {
        private readonly IManagerService _managerService;
        private readonly IMapper _mapper;

        public ManagersController(IManagerService managerService, IMapper mapper)
        {
            _managerService = managerService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetManagers(Guid hotelId)
        {
            var managers = await _managerService.GetManagersByHotelIdAsync(hotelId);
            var dtos = _mapper.Map<IEnumerable<ManagerResponseDto>>(managers);
            return Ok(ApiResponse<IEnumerable<ManagerResponseDto>>.Ok(dtos));
        }

        [HttpPost("{managerId}")]
        public async Task<IActionResult> AssignToHotel(Guid hotelId, string managerId)
        {
            var success = await _managerService.AssignManagerToHotelAsync(managerId, hotelId);
            if (!success)
                return BadRequest(ApiResponse<object>.Fail("Could not assign manager to hotel. Manager not found."));

            return Ok(ApiResponse<object>.Ok(null, "Manager assigned successfully."));
        }

        [HttpPut("{managerId}")]
        public async Task<IActionResult> Update(string managerId, [FromBody] ManagerUpdateDto dto)
        {
            var success = await _managerService.UpdateManagerAsync(managerId, dto);
            if (!success)
                return NotFound(ApiResponse<object>.Fail("Manager not found."));

            return NoContent();
        }

        [HttpDelete("{managerId}")]
        public async Task<IActionResult> Delete(string managerId)
        {
            var success = await _managerService.DeleteManagerAsync(managerId);
            if (!success)
                return BadRequest(ApiResponse<object>.Fail("Cannot delete: manager not found or is the last manager of this hotel."));

            return NoContent();
        }
    }
}