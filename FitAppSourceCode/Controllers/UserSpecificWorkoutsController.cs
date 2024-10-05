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
using Microsoft.Extensions.Logging;


namespace FitApp.Controllers
{
    [Authorize]
    public class UserSpecificWorkoutsController : Controller
    {
        private readonly FitAppContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<UserSpecificWorkoutsController> _logger;
        public UserSpecificWorkoutsController(FitAppContext context, UserManager<IdentityUser> userManager, ILogger<UserSpecificWorkoutsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // USER-SPECIFIC WORKOUTS OPERATIONS (for all users)
        // GET: Workouts (All users can view the workouts)
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            var userId = _userManager.GetUserId(User); // Fetching current user
            var userWorkouts = from w in _context.UserSpecificWorkouts
                               where w.UserId == userId
                               select w;

            // Search functionality
            if (!string.IsNullOrEmpty(searchString))
            {
                userWorkouts = userWorkouts.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper()));
            }

            // Sorting functionality
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CaloriesSortParm"] = sortOrder == "Calories" ? "calories_desc" : "Calories";
            ViewData["DurationSortParm"] = sortOrder == "Duration" ? "duration_desc" : "Duration";

            switch (sortOrder)
            {
                case "name_desc":
                    userWorkouts = userWorkouts.OrderByDescending(w => w.Name);
                    break;
                case "Calories":
                    userWorkouts = userWorkouts.OrderBy(w => w.CaloriesBurned);
                    break;
                case "calories_desc":
                    userWorkouts = userWorkouts.OrderByDescending(w => w.CaloriesBurned);
                    break;
                case "Duration":
                    userWorkouts = userWorkouts.OrderBy(w => w.Duration);
                    break;
                case "duration_desc":
                    userWorkouts = userWorkouts.OrderByDescending(w => w.Duration);
                    break;
                default:
                    userWorkouts = userWorkouts.OrderBy(w => w.Name);
                    break;
            }

            return View("MyWorkouts", await userWorkouts.ToListAsync());
        }

        // GET: Workouts/CreateUserWorkout (For logged-in users)
        [Authorize]
        public IActionResult CreateUserWorkout()
        {
            return View();
        }

        // POST: Workouts/CreateUserWorkout (For logged-in users)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CreateUserWorkout([Bind("Name,Description,Duration,CaloriesBurned")] UserSpecificWorkout userWorkout)
        {
            if (ModelState.IsValid)
            {
                userWorkout.UserId = _userManager.GetUserId(User);

                // Log the creation of a new workout
                _logger.LogInformation("Creating UserSpecificWorkout for UserId: {UserId}, Name: {Name}, Description: {Description}, Duration: {Duration}, CaloriesBurned: {CaloriesBurned}",
                    userWorkout.UserId, userWorkout.Name, userWorkout.Description, userWorkout.Duration, userWorkout.CaloriesBurned);

                _context.UserSpecificWorkouts.Add(userWorkout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyWorkouts));
            }

            // Log the validation failure
            _logger.LogWarning("CreateUserWorkout model state invalid. Model: {Model}", ModelState);
            return View(userWorkout);
        }

        // GET: Workouts/EditUserWorkout/5 (For logged-in users)
        [Authorize]
        public async Task<IActionResult> EditUserWorkout(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var userWorkout = await _context.UserSpecificWorkouts
                .FirstOrDefaultAsync(uw => uw.Id == id && uw.UserId == userId);

            if (userWorkout == null)
            {
                return NotFound();
            }

            return View(userWorkout);
        }

        // POST: Workouts/EditUserWorkout/5 (For logged-in users)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditUserWorkout(int id, [Bind("Id,Name,Description,Duration,CaloriesBurned")] UserSpecificWorkout userWorkout)
        {
            if (id != userWorkout.Id)
            {
                _logger.LogWarning("Workout ID mismatch: passed Id {Id} does not match workout Id {WorkoutId}", id, userWorkout.Id);
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (userWorkout.UserId != userId)
            {
                _logger.LogWarning("Unauthorized edit attempt. UserId: {UserId}, WorkoutId: {WorkoutId}", userId, userWorkout.Id);
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Log the workout update
                    _logger.LogInformation("Editing UserSpecificWorkout for UserId: {UserId}, WorkoutId: {WorkoutId}, Name: {Name}, Description: {Description}, Duration: {Duration}, CaloriesBurned: {CaloriesBurned}",
                        userWorkout.UserId, userWorkout.Id, userWorkout.Name, userWorkout.Description, userWorkout.Duration, userWorkout.CaloriesBurned);

                    _context.Update(userWorkout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!UserSpecificWorkoutExists(userWorkout.Id))
                    {
                        _logger.LogError(ex, "Concurrency error while updating non-existing UserSpecificWorkout with Id: {WorkoutId}", userWorkout.Id);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, "Concurrency error while updating UserSpecificWorkout with Id: {WorkoutId}", userWorkout.Id);
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyWorkouts));
            }

            // Log validation failure
            _logger.LogWarning("EditUserWorkout model state invalid for WorkoutId: {WorkoutId}. Model: {Model}", userWorkout.Id, ModelState);
            return View(userWorkout);
        }


        // GET: Workouts/DeleteUserWorkout/5 (For logged-in users)
        [Authorize]
        public async Task<IActionResult> DeleteUserWorkout(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var userWorkout = await _context.UserSpecificWorkouts
                .FirstOrDefaultAsync(uw => uw.Id == id && uw.UserId == userId);

            if (userWorkout == null)
            {
                return NotFound();
            }

            return View(userWorkout);
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
                return NotFound();
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
                return NotFound();
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

            return RedirectToAction(nameof(MyWorkouts));
        }

        [Authorize]
        // GET: Workouts/Details/5 (Users can view their own workout details)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userWorkout = await _context.UserSpecificWorkouts
                .FirstOrDefaultAsync(m => m.Id == id);

            if (userWorkout == null)
            {
                return NotFound();
            }

            return View(userWorkout);
        }

        private bool UserSpecificWorkoutExists(int id)
        {
            return _context.UserSpecificWorkouts.Any(e => e.Id == id);
        }
    }
}
