using daloy_api.Data;
using daloy_api.DTOs.admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace daloy_api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminDashboardController(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // KPI OVERVIEW
        // =====================================================
        [HttpGet("kpis")]
        public async Task<IActionResult> GetKpis()
        {
            var kpis = await _context.AdminDashboardKpis    
                .AsNoTracking()
                .FirstAsync();

            return Ok(kpis);
        }

        [HttpGet("charts")]
        public async Task<IActionResult> GetCharts(
            string view = "weekly",
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            object from = fromDate ?? (object)DBNull.Value;
            object to = toDate ?? (object)DBNull.Value;
            var weekly = await _context
                .Set<ChartRowDto>()
                .FromSqlRaw(
                    "EXEC sp_Admin_GetWeeklyActiveLearners @ViewType = {0}, @FromDate = {1}, @ToDate = {2}",
                    view,
                    from,
                    to
                )
                .AsNoTracking()
                .ToListAsync();

            var quizAverage = await _context
                .Set<ChartRowDto>()
                .FromSqlRaw("EXEC sp_Admin_GetAverageQuizScore")
                .AsNoTracking()
                .ToListAsync();

            var moduleCompletion = await _context
                .Set<ChartRowDto>()
                .FromSqlRaw("EXEC sp_Admin_GetModuleCompletionDistribution")
                .AsNoTracking()
                .ToListAsync();

            var videoCompletion = await _context
                .Set<ChartRowDto>()
                .FromSqlRaw("EXEC sp_Admin_GetVideoCompletionDistribution")
                .AsNoTracking()
                .ToListAsync();

            return Ok(new AdminDashboardChartsDto
            {
                WeeklyActiveLearners = new ChartSeriesDto
                {
                    Labels = weekly.Select(x => x.Label).ToList(),
                    Values = weekly.Select(x => x.Value).ToList()
                },
                AverageQuizPercentage = new ChartSeriesDto
                {
                    Labels = quizAverage.Select(x => x.Label).ToList(),
                    Values = quizAverage.Select(x => x.Value).ToList()
                },
                ModuleCompletionDistribution = new ChartSeriesDto
                {
                    Labels = moduleCompletion.Select(x => x.Label).ToList(),
                    Values = moduleCompletion.Select(x => x.Value).ToList()
                },
                VideoCompletionDistribution = new ChartSeriesDto
                {
                    Labels = videoCompletion.Select(x => x.Label).ToList(),
                    Values = videoCompletion.Select(x => x.Value).ToList()
                }
            });
        }


    }
}
