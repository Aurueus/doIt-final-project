using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMS.Application.DTO.Auth;
using Microsoft.AspNetCore.Identity;

namespace HMS.Application.Interfaces
{
    public interface IAuthService
{
    Task<IdentityResult> RegisterManager(RegistrationRequestDto dto);
    Task<IdentityResult> RegisterGuest(RegistrationRequestDto dto);
    Task<LoginResponseDto> Login(LoginRequestDto dto);
}
}