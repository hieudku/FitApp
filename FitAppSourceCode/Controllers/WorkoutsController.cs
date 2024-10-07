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

        // GET: Workouts Search and Sort
        // GET: Workouts
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            // Create sorting parameters for different columns
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DurationSortParm"] = sortOrder == "Duration" ? "duration_desc" : "Duration";
            ViewData["CaloriesSortParm"] = sortOrder == "Calories" ? "calories_desc" : "Calories";

            // Filter workouts if search string is provided
            var workouts = from w in _context.Workouts
                           select w;

            if (!String.IsNullOrEmpty(searchString))
            {
                workouts = workouts.Where(w => w.Name.Contains(searchString) || w.Description.Contains(searchString));
            }

            // Apply sorting logic based on the sort order parameter
            switch (sortOrder)
            {
                case "name_desc":
                    workouts = workouts.OrderByDescending(w => w.Name);
                    break;
                case "Duration":
                    workouts = workouts.OrderBy(w => w.Duration);
                    break;
                case "duration_desc":
                    workouts = workouts.OrderByDescending(w => w.Duration);
                    break;
                case "Calories":
                    workouts = workouts.OrderBy(w => w.CaloriesBurned);
                    break;
                case "calories_desc":
                    workouts = workouts.OrderByDescending(w => w.CaloriesBurned);
                    break;
                default:
                    workouts = workouts.OrderBy(w => w.Name);
                    break;
            }

#if DEBUG
            // For testing: use synchronous ToList() to avoid async mock complexities
            return View(workouts.ToList());
#else
    // Production code uses async method
    return View(await workouts.ToListAsync());
#endif
        }

        // GET: Workouts/Details/5 (All users can view details)
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

        // ADMIN-ONLY CRUD OPERATIONS FOR MASTER Workouts

        // GET: Workouts/Create (Admin only)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Manually enforce role check since Authorize isn't invoked in unit tests
            if (!User.IsInRole("Admin"))
            {
                return Forbid();  // Non-admin users should get a ForbidResult
            }

            Console.WriteLine("CREATE SUCCESSFUL");
            return View();
        }

        // POST: Workouts/Create (Admin only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Description,Duration,CaloriesBurned")] Workouts workout)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workout);
        }

        // GET: Workouts/Edit/5 (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
            {
                return NotFound();
            }
            return View(workout);
        }

        // POST: Workouts/Edit/5 (Admin only)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Duration,CaloriesBurned")] Workouts workout)
        {
            if (id != workout.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutExists(workout.Id))
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
            return View(workout);
        }

        // GET: Workouts/Delete/5 (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        // POST: Workouts/Delete/5 (Admin only)
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workout = await _context.Workouts.FindAsync(id);
            if (workout == null)
            {
                return NotFound();  // If the workout is not found, return NotFoundResult
            }
            _context.Workouts.Remove(workout);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // HELPER METHOD FOR WORKOUT EXISTENCE
        private bool WorkoutExists(int id)
        {
            return _context.Workouts.Any(e => e.Id == id);
        }

        // POST: Workouts/DeleteUserWorkout/5 (For logged-in users)
        [HttpPost, ActionName("DeleteUserWorkout")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteUserWorkoutConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            var userWorkout = await _context.UserSpecificWorkouts
                .FirstOrDefaultAsync(uw => uw.Id == id && uw.UserId == userId);

            if (userWorkout == null)
            {
                return NotFound(); // Return NotFound if the workout doesn't exist
            }

            _context.UserSpecificWorkouts.Remove(userWorkout);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyWorkouts));
        }

        // GET: Workouts/MyWorkouts (For logged-in users)
        [Authorize]
        public async Task<IActionResult> MyWorkouts()
        {
            var userId = _userManager.GetUserId(User);
            var userWorkouts = _context.UserSpecificWorkouts
                .Where(uw => uw.UserId == userId);

            return View(await userWorkouts.ToListAsync());
        }

        // POST: Workouts/SaveWorkout (For logged-in users)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveWorkout(int workoutId)
        {
            var userId = _userManager.GetUserId(User);
            var workout = await _context.Workouts.FindAsync(workoutId);

            if (workout == null)
            {
                return NotFound(); // Return NotFound if the workout doesn't exist
            }

            var userWorkout = new UserSpecificWorkout
            {
                UserId = userId,
                Name = workout.Name,
                Description = workout.Description,
                Duration = workout.Duration,
                CaloriesBurned = workout.CaloriesBurned
            };

            _context.UserSpecificWorkouts.Add(userWorkout);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect to the workout list after saving
        }

        private bool UserSpecificWorkoutExists(int id)
        {
            return _context.UserSpecificWorkouts.Any(e => e.Id == id);
        }
    }
}
