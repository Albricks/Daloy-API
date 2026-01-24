using daloy_api.Content.Videos;
using daloy_api.Models;
using daloy_api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace daloy_api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class VideosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IVideoService _videoService;

    public VideosController(
        AppDbContext db,
        IVideoService videoService)
    {
        _db = db;
        _videoService = videoService;
    }

    // --------------------
    // LIST videos by module
    // GET: /api/videos/module/{moduleId}
    // --------------------
    [HttpGet("module/{moduleId:guid}")]
    public async Task<IActionResult> GetByModule(Guid moduleId)
    {
        var videos = await _db.Videos
            .Where(v => v.LearningModuleId == moduleId)
            .OrderBy(v => v.Order)
            .Select(v => new
            {
                v.Id,
                v.Title,
                v.Description,
                v.Duration,
                v.Order
            })
            .ToListAsync();

        return Ok(videos);
    }

    // --------------------
    // WATCH video
    // GET: /api/videos/{videoId}
    // --------------------
    [HttpGet("{videoId:guid}")]
    public async Task<IActionResult> Watch(Guid videoId)
    {
        var video = await _db.Videos
            .FirstOrDefaultAsync(v => v.Id == videoId);

        if (video == null)
            return NotFound();

        var sasUrl = _videoService.GetVideoSasUrl(video.BlobName);

        return Ok(new
        {
            video.Id,
            video.Title,
            video.Description,
            video.Duration,
            streamUrl = sasUrl
        });
    }

    // --------------------
    // LIST ALL videos (for video list page)
    // GET: /api/videos
    // --------------------
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var videos = await _db.Videos
            .OrderBy(v => v.LearningModuleId)
            .ThenBy(v => v.Order)
            .ToListAsync();

        var result = videos.Select(v => new
        {
            Id = v.Id,
            Title = v.Title,

            Duration = v.Duration.HasValue
                ? v.Duration.Value.ToString(@"mm\:ss")
                : "00:00",

            Status = v.Status switch
            {
                VideoStatus.NotStarted => "not-started",
                VideoStatus.InProgress => "in-progress",
                VideoStatus.Completed => "completed",
                _ => "not-started"
            },

            ThumbnailUrl = !string.IsNullOrWhiteSpace(v.ThumbnailBlobName) ? _videoService.GetThumbnailUrl(v.ThumbnailBlobName) : _videoService.GetThumbnailUrl("modules/shared/thumbnails/default.jpg"),

            ModuleId = v.LearningModuleId,
            Order = v.Order
        });

        return Ok(result);
    }

}
