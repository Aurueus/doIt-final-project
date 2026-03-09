using HMS.Application.Common;
using HMS.Application.DTO.Auth;
using HMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HMS.Api.Controllers
{
    [Route("api/auth")]
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
            if (!result.Succeeded)
                return BadRequest(ApiResponse<object>.Fail(
                    string.Join("; ", result.Errors.Select(e => e.Description))));

            return Ok(ApiResponse<object>.Ok(null, "Manager registered successfully."));
        }

        [HttpPost("register/guest")]
        public async Task<IActionResult> RegisterGuest([FromBody] RegistrationRequestDto request)
        {
            var result = await _authService.RegisterGuest(request);
            if (!result.Succeeded)
                return BadRequest(ApiResponse<object>.Fail(
                    string.Join("; ", result.Errors.Select(e => e.Description))));

            return Ok(ApiResponse<object>.Ok(null, "Guest registered successfully."));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.Login(request);
            return Ok(ApiResponse<LoginResponseDto>.Ok(response, "Login successful."));
        }
    }
}