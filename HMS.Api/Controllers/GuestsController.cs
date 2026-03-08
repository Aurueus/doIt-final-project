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
    [Route("api/[controller]")]
    [ApiController]
    public class GuestsController : ControllerBase
    {
        private readonly IGuestService _guestService;

        public GuestsController(IGuestService guestService)
        {
            _guestService = guestService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProfile(string id)
        {
            var guest = await _guestService.GetGuestByIdAsync(id);
            if (guest == null) return NotFound();
            return Ok(guest);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, [FromBody] ApplicationUser guest)
        {
            try
            {
                var success = await _guestService.UpdateGuestAsync(id, guest);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (ArgumentException ex) { return BadRequest(new { Message = ex.Message }); }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _guestService.DeleteGuestAsync(id);
            if (!success) 
                return BadRequest(new { Message = "Cannot delete guest: Active or future reservations exist" });
            
            return NoContent();
        }
    }
}