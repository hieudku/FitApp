using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitApp.Data;
using FitApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FitApp.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly FitAppContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WorkoutsController(FitAppContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Workouts
        public async Task<IActionResult> Index(string searchString)
        {
            if (_context.Workouts == null)
            {
                return Problem("Entity set 'FitAppContext.Workouts' is null");
            }

            var workouts = from w in _context.Workouts
                           select w;

            if (!string.IsNullOrEmpty(searchString))
            {
                workouts = workouts.Where(s => s.Name!.ToUpper().Contains(searchString.ToUpper()));
            }

            return View(await workouts.ToListAsync());
        }

        // GET: Workouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workouts = await _context.Workouts
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workouts == null)
            {
                return NotFound();
            }

            return View(workouts);
        }

        // GET: Workouts/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Duration,CaloriesBurned")] Workouts workouts)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workouts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workouts);
        }

        // GET: Workouts/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workouts = await _context.Workouts.FindAsync(id);
            if (workouts == null)
            {
                return NotFound();
            }
            return View(workouts);
        }

        // POST: Workouts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Duration,CaloriesBurned")] Workouts workouts)
        {
            if (id != workouts.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workouts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutsExists(workouts.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workouts);
        }

        // GET: Workouts/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workouts = await _context.Workouts
                .FirstOrDefaultAsync(m => m.Id == id);

            if (workouts == null)
            {
                return NotFound();
            }

            return View(workouts);
        }

        // POST: Workouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workouts = await _context.Workouts.FindAsync(id);
            if (workouts != null)
            {
                _context.Workouts.Remove(workouts);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Workouts/SaveWorkout
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveWorkout(int workoutId)
        {
            var userId = _userManager.GetUserId(User);
            var userWorkout = new UserWorkout
            {
                UserId = userId,
                WorkoutId = workoutId
            };

            _context.UserWorkouts.Add(userWorkout);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutsExists(int id)
        {
            return _context.Workouts.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> MyWorkouts()
        {
            var userId = _userManager.GetUserId(User);
            var userWorkouts = _context.UserWorkouts
                .Include(uw => uw.Workout)
                .Where(uw => uw.UserId == userId);

            return View(await userWorkouts.ToListAsync());
        }
    }
}
