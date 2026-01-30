using daloy_api.Models;
using System.Security.Claims;

namespace daloy_api.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser user);   // 👈 async for roles
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
