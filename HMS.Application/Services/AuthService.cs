using HMS.Application.DTO.Auth;
using HMS.Application.Interfaces;
using HMS.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwt)
        {
            _userManager = userManager;
            _jwt = jwt;
        }

        public async Task<IdentityResult> RegisterManager(RegistrationRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.PersonalNumber) || dto.PersonalNumber.Length != 11)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Personal Number must be 11 characters long" });
            }

            var exists = await _userManager.Users.AnyAsync(u =>
                u.PersonalNumber == dto.PersonalNumber || u.Email == dto.Email);

            if (exists)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Personal Number or Email already registered" });
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PersonalNumber = dto.PersonalNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Manager");
            }
            return result;
        }

        public async Task<IdentityResult> RegisterGuest(RegistrationRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.PersonalNumber) || dto.PersonalNumber.Length != 11)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Personal Number must be 11 characters long" });
            }

            var exists = await _userManager.Users.AnyAsync(u =>
                u.PersonalNumber == dto.PersonalNumber ||
                u.PhoneNumber == dto.PhoneNumber ||
                u.Email == dto.Email);

            if (exists)
            {
                return IdentityResult.Failed(new IdentityError { Description = "ID, Email, or Phone Number already registered" });
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PersonalNumber = dto.PersonalNumber,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Guest");
            }
            return result;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                throw new Exception("Invalid Credentials");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwt.GenerateToken(user, roles);

            return new LoginResponseDto
            {
                Token = token,
                Email = user.Email!
            };
        }
    }
}