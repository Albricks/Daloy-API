using daloy_api.DTOs;
using daloy_api.Models;
using daloy_api.Services;
using daloy_api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace daloy_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ProfileController : ControllerBase
{
    private readonly IAvatarService _avatarService;
    private readonly UserManager<AppUser> _userManager;

    public ProfileController(
        IAvatarService avatarService,
        UserManager<AppUser> userManager,
        BlobStorageService blobService)
    {
        _avatarService = avatarService;
        _userManager = userManager;
    }

    [HttpPut("me")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateMe([FromForm] UpdateProfileRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Unauthorized();

        // --------------------
        // Update basic fields
        // --------------------
        user.FullName = request.FullName;
        user.BirthDate = request.BirthDate;

        // --------------------
        // Update avatar (optional)
        // --------------------
        if (request.Avatar != null)
        {
            // 1. Validate
            _avatarService.ValidateAvatar(request.Avatar);

            // 2. Delete old avatar
            await _avatarService.DeleteAvatarIfExistsAsync(user.AvatarBlobName);

            // 3. Upload new avatar
            var blobName = await _avatarService
                .UploadAvatarAsync(request.Avatar, user.Id.ToString());

            user.AvatarBlobName = blobName;
        }

        // --------------------
        // Save user
        // --------------------
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new
            {
                message = "Failed to update profile",
                errors
            });
        }

        return Ok(new
        {
            message = "Profile updated successfully"
        });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut("avatar")]
    public async Task<IActionResult> UpdateAvatar(IFormFile avatar)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Unauthorized();

        // 1. Validate
        _avatarService.ValidateAvatar(avatar);

        // 2. Delete old avatar
        await _avatarService.DeleteAvatarIfExistsAsync(user.AvatarBlobName);

        // 3. Upload new avatar
        var newBlobName = await _avatarService.UploadAvatarAsync(avatar, userId);

        // 4. Save blob name to DB
        user.AvatarBlobName = newBlobName;
        await _userManager.UpdateAsync(user);

        // 5. Return SAS URL
        var sasUrl = _avatarService.GetAvatarSasUrl(newBlobName);

        return Ok(new
        {
            avatarUrl = sasUrl
        });
    }


}
