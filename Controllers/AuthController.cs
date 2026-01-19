using daloy_api.DTOs;
using daloy_api.Models;
using daloy_api.Responses;
using daloy_api.Services;
using daloy_api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace daloy_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    // --------------------
    // REGISTER
    // --------------------
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.Fail("Invalid registration data."));
        }

        var user = new AppUser
        {
            Email = request.Email.ToLower(),
            UserName = request.Username.ToLower(),
            FullName = request.FullName,
            BirthDate = request.BirthDate
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            // Collect Identity error messages
            var errors = result.Errors
                .Select(e => e.Description)
                .ToList();

            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Registration failed",
                Data = errors
            });
        }

        return Ok(ApiResponse<object>.Ok(null, "Registration successful"));
    }

    // --------------------
    // LOGIN
    // --------------------
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.ToLower());
        if (user == null)
            return Unauthorized(ApiResponse<AuthResponse>.Fail("Invalid credentials"));

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            false
        );

        if (!result.Succeeded)
            return Unauthorized(ApiResponse<AuthResponse>.Fail("Invalid credentials"));

        var accessToken = _tokenService.CreateToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        return Ok(ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FullName = user.FullName,
                BirthDate = user.BirthDate
            }
        }));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Unauthorized();

        return Ok(new
        {
            user.Id,
            user.Email,
            user.UserName,
            user.FullName,
            user.BirthDate
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Refresh(RefreshRequest request)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            return Unauthorized(ApiResponse<AuthResponse>.Fail("Invalid token"));

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null ||
            user.RefreshToken != request.RefreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return Unauthorized(ApiResponse<AuthResponse>.Fail("Invalid refresh token"));
        }

        var newAccessToken = _tokenService.CreateToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return Ok(ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FullName = user.FullName,
                BirthDate = user.BirthDate
            }
        }));
    }


}
