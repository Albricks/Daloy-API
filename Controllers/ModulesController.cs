using daloy_api.Content.Modules;
using daloy_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace daloy_api.Controllers
{
    [ApiController]
    [Route("api/modules")]
    public class ModulesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly BlobStorageService _blobStorage;

        public ModulesController(AppDbContext db, BlobStorageService blobStorage)
        {
            _db = db;
            _blobStorage = blobStorage;
        }

        [HttpGet]
        public async Task<IActionResult> GetModules()
        {
            var userId = /* get from auth context later */ Guid.Empty;

            var modules = await _db.Modules
                .OrderBy(m => m.Order)
                .Select(m => new ModuleListDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    Level = m.Level,
                    Duration = m.DurationMinutes + " mins",
                    Order = m.Order,

                    Status = _db.UserModuleProgresses
                        .Where(p => p.ModuleId == m.Id && p.UserId == userId)
                        .Select(p => p.Status.ToString())
                        .FirstOrDefault() ?? "New",

                    Progress = (int)_db.UserModuleProgresses
                        .Where(p => p.ModuleId == m.Id && p.UserId == userId)
                        .Select(p => p.ProgressPercent)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(modules);
        }

        // PREVIEW PAGE
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetModule(Guid id)
        {
            var userId = /* get from auth context later */ Guid.Empty;

            var module = await _db.Modules
                .OrderBy(m => m.Order)
                .Select(m => new ModulePreviewDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description,
                    Level = m.Level,
                    Duration = m.DurationMinutes + " mins",
                    Lessons = _db.Lessons.Count(l => l.ModuleId == m.Id),

                    Status = _db.UserModuleProgresses
                        .Where(p => p.ModuleId == m.Id && p.UserId == userId)
                        .Select(p => p.Status.ToString())
                        .FirstOrDefault() ?? "New",

                    Objectives = _db.ModuleObjectives
                        .Where(o => o.ModuleId == m.Id)
                        .OrderBy(o => o.Order)
                        .Select(o => o.Text)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (module == null)
                return NotFound();

            return Ok(module);
        }

        // READ PAGE
        [HttpGet("{id:guid}/lessons")]
        public async Task<IActionResult> GetLessons(Guid id)
        {
            var containerSas = _blobStorage.GetModulesContainerSas();
            var lessons = await _db.Lessons
                .Where(l => l.ModuleId == id)
                .OrderBy(l => l.Order)
                .Select(l => new LessonDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Order = l.Order,
                    EstimatedMinutes = l.EstimatedMinutes,
                    LessonType = l.LessonType,
                    ContainerSas = containerSas,
                    ContentUrl = _blobStorage.GetLessonSasUrl(l.ContentBlobPath)
                })
                .ToListAsync();

            return Ok(lessons);
        }   
    }
}