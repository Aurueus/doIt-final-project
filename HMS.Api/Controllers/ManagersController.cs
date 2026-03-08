using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Application.Interfaces;
using HMS.Core.Models;
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

        public ManagersController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetManagers(Guid hotelId)
        {
            var managers = await _managerService.GetManagersByHotelIdAsync(hotelId);
            return Ok(managers);
        }

        [HttpPost("{managerId}")]
        public async Task<IActionResult> AssignToHotel(Guid hotelId, string managerId)
        {
            var success = await _managerService.AssignManagerToHotelAsync(managerId, hotelId);
            if (!success) return BadRequest("Could not assign manager to hotel");
            return Ok("Manager assigned successfully.");
        }

        [HttpDelete("{managerId}")]
        public async Task<IActionResult> DeleteManager(string managerId)
        {
            var success = await _managerService.DeleteManagerAsync(managerId);
            if (!success) 
                return BadRequest("Operation failed. Manager not found or is the LAST manager for this hotel");
            
            return NoContent();
        }
    }
}