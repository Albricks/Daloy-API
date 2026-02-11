using daloy_api.Data;
using daloy_api.DTOs.admin;
using daloy_api.DTOs.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace daloy_api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminUsersController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Learner 360° overview:
        /// Modules + Situational + Quiz + Video (aggregated)
        /// </summary>
        [HttpGet("{userId}/overall-progress")]
        public async Task<IActionResult> GetUserOverallProgress(Guid userId)
        {
            var result = await _context
                .Set<AdminUserOverallProgressDto>()
                .FromSqlRaw(
                    "EXEC sp_Admin_GetUserOverallProgress @UserId = {0}",
                    userId
                )
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers([FromQuery] string? search)
        {
            object searchParam = string.IsNullOrWhiteSpace(search)
                ? DBNull.Value
                : search;

            var result = await _context
                .Set<AdminUserListDto>()
                .FromSqlRaw(
                    "EXEC sp_Admin_SearchUsers @Search = {0}",
                    searchParam
                )
                .ToListAsync();

            return Ok(result);
        }
    }
}
