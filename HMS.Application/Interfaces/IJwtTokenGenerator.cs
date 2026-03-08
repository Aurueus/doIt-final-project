using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HMS.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HMS.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser user, IEnumerable<string> roles);
    }
}