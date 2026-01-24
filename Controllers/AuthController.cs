using daloy_api.DTOs;
using daloy_api.Models;
using daloy_api.Responses;
using daloy_api.Services;
using daloy_api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace daloy_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAvatarService _avatarService;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthController(
        IAvatarService avatarService,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService

    )
    {
        _avatarService = avatarService;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    // --------------------
    // REGISTER
    // --------------------
    [HttpPost("register")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Register(
    [FromForm] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(
                ApiResponse<object>.Fail("Invalid registration data."));
        }

        var userId = Guid.NewGuid();

        var user = new AppUser
        {
            Id = userId, // 👈 IMPORTANT
            Email = request.Email.ToLower(),
            UserName = request.Username.ToLower(),
            FullName = request.FullName,
            BirthDate = request.BirthDate
        };

        // Upload avatar BEFORE creating user
        if (request.Avatar != null)
        {
            _avatarService.ValidateAvatar(request.Avatar);

            user.AvatarBlobName = await _avatarService
                .UploadAvatarAsync(request.Avatar, user.Id.ToString());
        }

        var result = await _userManager
            .CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
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

        // Generate SAS URL (optional)
        string? avatarUrl = user.AvatarBlobName != null
            ? _avatarService.GetAvatarSasUrl(user.AvatarBlobName)
            : null;

        return Ok(ApiResponse<object>.Ok(new
        {
            Message = "Registration successful",
            UserId = user.Id,
            AvatarUrl = avatarUrl
        }));
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

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("me")]
    public async Task<ActionResult<MeDto>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Unauthorized();

        string? avatarUrl = null;

        if (!string.IsNullOrEmpty(user.AvatarBlobName))
        {
            avatarUrl = _avatarService.GetAvatarSasUrl(user.AvatarBlobName);
        }

        return Ok(new MeDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!,
            FullName = user.FullName,
            BirthDate = user.BirthDate,
            AvatarUrl = avatarUrl
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
