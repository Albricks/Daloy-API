using daloy_api.Content.Diary;
using daloy_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace daloy_api.Controllers
{
    [ApiController]
    [Route("api/budget-diary")]
    [Authorize]
    public class BudgetDiaryController : ControllerBase
    {
        private readonly AppDbContext _db;

        public BudgetDiaryController(AppDbContext db)
        {
            _db = db;
        }

        private Guid GetUserId()
        {
            // Match your existing JWT pattern
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }

        // ============================
        // GET: api/budget-diary
        // ============================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();

            var entries = await _db.BudgetDiaryEntries
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EntryDate)
                .Select(e => new BudgetDiaryEntryDto
                {
                    Id = e.Id,
                    Date = e.EntryDate.ToString("yyyy-MM-dd"),
                    Budget = e.Budget,
                    Spent = e.Spent,
                    Notes = e.Notes,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToListAsync();

            return Ok(entries);
        }

        // ============================
        // POST: api/budget-diary
        // UPSERT by (UserId + EntryDate)
        // ============================
        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] UpsertBudgetDiaryEntryRequest request)
        {
            var userId = GetUserId();

            var entryDate = request.EntryDate.Date;

            var existing = await _db.BudgetDiaryEntries
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId &&
                    e.EntryDate == entryDate);

            if (existing != null)
            {
                // UPDATE
                existing.Budget = request.Budget;
                existing.Spent = request.Spent;
                existing.Notes = request.Notes;
                existing.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();

                return Ok(new BudgetDiaryEntryDto
                {
                    Id = existing.Id,
                    Date = existing.EntryDate.ToString("yyyy-MM-dd"),
                    Budget = existing.Budget,
                    Spent = existing.Spent,
                    Notes = existing.Notes,
                    CreatedAt = existing.CreatedAt,
                    UpdatedAt = existing.UpdatedAt
                });
            }

            // CREATE
            var created = new BudgetDiaryEntry
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                EntryDate = entryDate,
                Budget = request.Budget,
                Spent = request.Spent,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.BudgetDiaryEntries.Add(created);
            await _db.SaveChangesAsync();

            return Ok(new BudgetDiaryEntryDto
            {
                Id = created.Id,
                Date = created.EntryDate.ToString("yyyy-MM-dd"),
                Budget = created.Budget,
                Spent = created.Spent,
                Notes = created.Notes,
                CreatedAt = created.CreatedAt,
                UpdatedAt = created.UpdatedAt
            });
        }

        // ============================
        // DELETE: api/budget-diary/{id}
        // ============================
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();

            var entry = await _db.BudgetDiaryEntries
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (entry == null)
                return NotFound();

            _db.BudgetDiaryEntries.Remove(entry);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // ============================
        // GET: api/budget-diary/summary/weekly
        // ============================
        [HttpGet("summary/weekly")]
        public async Task<IActionResult> GetWeeklySummary()
        {
            var userId = GetUserId();

            var today = DateTime.UtcNow.Date;

            // Start of week (Monday)
            var diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var weekStart = today.AddDays(-diff);
            var weekEnd = weekStart.AddDays(7);

            var query = _db.BudgetDiaryEntries
                .Where(e =>
                    e.UserId == userId &&
                    e.EntryDate >= weekStart &&
                    e.EntryDate < weekEnd);

            var summary = await query
                .GroupBy(_ => 1)
                .Select(g => new BudgetDiarySummaryDto
                {
                    TotalBudget = g.Sum(x => x.Budget),
                    TotalSpent = g.Sum(x => x.Spent),
                    TotalSaved = g.Sum(x => x.Saved),
                    EntryCount = g.Count()
                })
                .FirstOrDefaultAsync()
                ?? new BudgetDiarySummaryDto();

            return Ok(summary);
        }

        // ============================
        // GET: api/budget-diary/summary/monthly
        // ============================
        [HttpGet("summary/monthly")]
        public async Task<IActionResult> GetMonthlySummary()
        {
            var userId = GetUserId();

            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var monthEnd = monthStart.AddMonths(1);

            var query = _db.BudgetDiaryEntries
                .Where(e =>
                    e.UserId == userId &&
                    e.EntryDate >= monthStart &&
                    e.EntryDate < monthEnd);

            var summary = await query
                .GroupBy(_ => 1)
                .Select(g => new BudgetDiarySummaryDto
                {
                    TotalBudget = g.Sum(x => x.Budget),
                    TotalSpent = g.Sum(x => x.Spent),
                    TotalSaved = g.Sum(x => x.Saved),
                    EntryCount = g.Count()
                })
                .FirstOrDefaultAsync()
                ?? new BudgetDiarySummaryDto();

            return Ok(summary);
        }

        // ============================
        // GET: api/budget-diary/export/csv
        // ============================
        [HttpGet("export/csv")]
        public async Task<IActionResult> ExportCsv()
        {
            var userId = GetUserId();

            var entries = await _db.BudgetDiaryEntries
                .Where(e => e.UserId == userId)
                .OrderBy(e => e.EntryDate)
                .ToListAsync();

            var sb = new StringBuilder();

            // CSV Header
            sb.AppendLine("Date,Budget,Spent,Saved,Notes,CreatedAt,UpdatedAt");

            foreach (var e in entries)
            {
                var notes = (e.Notes ?? "").Replace("\"", "\"\"");

                sb.AppendLine(
                    $"{e.EntryDate:yyyy-MM-dd}," +
                    $"{e.Budget}," +
                    $"{e.Spent}," +
                    $"{e.Saved}," +
                    $"\"{notes}\"," +
                    $"{e.CreatedAt:O}," +
                    $"{e.UpdatedAt:O}"
                );
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"budget-diary-{DateTime.UtcNow:yyyyMMdd}.csv";

            return File(bytes, "text/csv", fileName);
        }

        // ============================
        // GET: api/budget-diary/export/csv?from=2026-01-01&to=2026-01-31
        // ============================
        [HttpGet("export/csv-range")]
        public async Task<IActionResult> ExportCsvRange(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var userId = GetUserId();

            var entries = await _db.BudgetDiaryEntries
                .Where(e =>
                    e.UserId == userId &&
                    e.EntryDate >= from.Date &&
                    e.EntryDate <= to.Date)
                .OrderBy(e => e.EntryDate)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Date,Budget,Spent,Saved,Notes");

            foreach (var e in entries)
            {
                var notes = (e.Notes ?? "").Replace("\"", "\"\"");

                sb.AppendLine(
                    $"{e.EntryDate:yyyy-MM-dd}," +
                    $"{e.Budget}," +
                    $"{e.Spent}," +
                    $"{e.Saved}," +
                    $"\"{notes}\""
                );
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"budget-diary-{from:yyyyMMdd}-to-{to:yyyyMMdd}.csv";

            return File(bytes, "text/csv", fileName);
        }

    }
}
