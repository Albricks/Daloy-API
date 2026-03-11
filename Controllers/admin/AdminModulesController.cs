using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using daloy_api.DTOs.admin;
using daloy_api.Data;

namespace daloy_api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/modules")]
    [Authorize(Roles = "Admin")]
    public class AdminModulesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminModulesController(AppDbContext context)
        {
            _context = context;
        }

        // =============================================
        // MODULE LIST (for filter dropdown)
        // =============================================
        [HttpGet]
        public async Task<IActionResult> GetModules()
        {
            var modules = await _context.Modules
                .OrderBy(m => m.Order)
                .Select(m => new
                {
                    id = m.Id,
                    title = m.Title
                })
                .ToListAsync();

            return Ok(modules);
        }

        // =============================================
        // MODULE PROGRESS
        // =============================================
        [HttpGet("{moduleId}/progress")]
        public async Task<IActionResult> GetModuleProgress(Guid moduleId)
        {
            var result = await _context
                .Set<AdminModuleProgressDto>()
                .FromSqlRaw("EXEC sp_Admin_GetModuleProgress @ModuleId = {0}", moduleId)
                .ToListAsync();

            return Ok(result);
        }
    }
}