using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BeFit.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        public async Task<IActionResult> Index()
        {
            string userId = GetCurrentUserId();
            DateTime fromDate = DateTime.Now.AddDays(-28);

            var statistics = await _context.Exercises
                .Include(e => e.ExerciseType)
                .Include(e => e.Session)
                .Where(e => e.UserId == userId
                            && e.Session != null
                            && e.Session.Start >= fromDate)
                .GroupBy(e => e.ExerciseType!.Name)
                .Select(g => new ExerciseStatisticsViewModel
                {
                    ExerciseTypeName = g.Key,
                    TimesPerformed = g.Count(),
                    TotalReps = g.Sum(x => x.NumOfSeries * x.NumOfReps),
                    AverageWeight = g.Average(x => x.Weight),
                    MaxWeight = g.Max(x => x.Weight)
                })
                .OrderBy(x => x.ExerciseTypeName)
                .ToListAsync();

            return View(statistics);
        }
    }
}