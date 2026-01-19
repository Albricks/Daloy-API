using daloy_api.Models;
using System.Security.Claims;

namespace daloy_api.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
