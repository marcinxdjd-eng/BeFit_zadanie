using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BeFit.Controllers
{
    [Authorize]
    public class ExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExercisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        private async Task PopulateDropDownsAsync(int? selectedExerciseTypeId = null, int? selectedSessionId = null)
        {
            string userId = GetCurrentUserId();

            var exerciseTypes = await _context.ExerciseTypes
                .OrderBy(t => t.Name)
                .ToListAsync();

            var sessions = await _context.Sessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.Start)
                .ToListAsync();

            ViewData["ExerciseTypeId"] = new SelectList(
                exerciseTypes,
                "Id",
                "Name",
                selectedExerciseTypeId);

            ViewData["SessionId"] = new SelectList(
                sessions.Select(s => new
                {
                    s.Id,
                    Display = $"{s.Start:yyyy-MM-dd HH:mm} - {s.End:yyyy-MM-dd HH:mm}"
                }),
                "Id",
                "Display",
                selectedSessionId);
        }

        // GET: Exercises
        public async Task<IActionResult> Index()
        {
            string userId = GetCurrentUserId();

            var exercises = await _context.Exercises
                .Include(e => e.ExerciseType)
                .Include(e => e.Session)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Session!.Start)
                .ThenBy(e => e.ExerciseType!.Name)
                .ToListAsync();

            return View(exercises);
        }

        // GET: Exercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string userId = GetCurrentUserId();

            var exercise = await _context.Exercises
                .Include(e => e.ExerciseType)
                .Include(e => e.Session)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (exercise == null)
            {
                return NotFound();
            }

            return View(exercise);
        }

        // GET: Exercises/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropDownsAsync();
            return View();
        }

        // POST: Exercises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Weight,NumOfSeries,NumOfReps,ExerciseTypeId,SessionId")] Exercise exercise)
        {
            string userId = GetCurrentUserId();
            exercise.UserId = userId;

            bool exerciseTypeExists = await _context.ExerciseTypes
                .AnyAsync(t => t.Id == exercise.ExerciseTypeId);

            if (!exerciseTypeExists)
            {
                ModelState.AddModelError("ExerciseTypeId", "Wybrany typ ćwiczenia nie istnieje.");
            }

            bool sessionExists = await _context.Sessions
                .AnyAsync(s => s.Id == exercise.SessionId && s.UserId == userId);

            if (!sessionExists)
            {
                ModelState.AddModelError("SessionId", "Wybrana sesja nie istnieje lub nie należy do użytkownika.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(exercise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropDownsAsync(exercise.ExerciseTypeId, exercise.SessionId);
            return View(exercise);
        }

        // GET: Exercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string userId = GetCurrentUserId();

            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (exercise == null)
            {
                return NotFound();
            }

            await PopulateDropDownsAsync(exercise.ExerciseTypeId, exercise.SessionId);
            return View(exercise);
        }

        // POST: Exercises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Weight,NumOfSeries,NumOfReps,ExerciseTypeId,SessionId")] Exercise formExercise)
        {
            if (id != formExercise.Id)
            {
                return NotFound();
            }

            string userId = GetCurrentUserId();

            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (exercise == null)
            {
                return NotFound();
            }

            bool exerciseTypeExists = await _context.ExerciseTypes
                .AnyAsync(t => t.Id == formExercise.ExerciseTypeId);

            if (!exerciseTypeExists)
            {
                ModelState.AddModelError("ExerciseTypeId", "Wybrany typ ćwiczenia nie istnieje.");
            }

            bool sessionExists = await _context.Sessions
                .AnyAsync(s => s.Id == formExercise.SessionId && s.UserId == userId);

            if (!sessionExists)
            {
                ModelState.AddModelError("SessionId", "Wybrana sesja nie istnieje lub nie należy do użytkownika.");
            }

            if (ModelState.IsValid)
            {
                exercise.Weight = formExercise.Weight;
                exercise.NumOfSeries = formExercise.NumOfSeries;
                exercise.NumOfReps = formExercise.NumOfReps;
                exercise.ExerciseTypeId = formExercise.ExerciseTypeId;
                exercise.SessionId = formExercise.SessionId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropDownsAsync(formExercise.ExerciseTypeId, formExercise.SessionId);
            return View(formExercise);
        }

        // GET: Exercises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string userId = GetCurrentUserId();

            var exercise = await _context.Exercises
                .Include(e => e.ExerciseType)
                .Include(e => e.Session)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (exercise == null)
            {
                return NotFound();
            }

            return View(exercise);
        }

        // POST: Exercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string userId = GetCurrentUserId();

            var exercise = await _context.Exercises
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (exercise == null)
            {
                return NotFound();
            }

            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}