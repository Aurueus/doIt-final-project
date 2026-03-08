using HMS.Application.DTO.Auth;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register/manager")]
        public async Task<IActionResult> RegisterManager([FromBody] RegistrationRequestDto request)
        {
            var result = await _authService.RegisterManager(request);
            if (!result.Succeeded) return BadRequest(result.Errors);
            
            return Ok("Manager registered successfully");
        }

        [HttpPost("register/guest")]
        public async Task<IActionResult> RegisterGuest([FromBody] RegistrationRequestDto request)
        {
            var result = await _authService.RegisterGuest(request);
            if (!result.Succeeded) return BadRequest(result.Errors);
            
            return Ok("Guest registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try 
            {
                var response = await _authService.Login(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}