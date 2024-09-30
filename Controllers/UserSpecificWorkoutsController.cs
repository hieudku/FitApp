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
    [Authorize]
    public class UserSpecificWorkoutsController : Controller
    {
        private readonly FitAppContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UserSpecificWorkoutsController(FitAppContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // USER-SPECIFIC WORKOUTS OPERATIONS (for all users)

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
                _context.UserSpecificWorkouts.Add(userWorkout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyWorkouts));
            }
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
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (userWorkout.UserId != userId)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userWorkout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserSpecificWorkoutExists(userWorkout.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyWorkouts));
            }
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

        private bool UserSpecificWorkoutExists(int id)
        {
            return _context.UserSpecificWorkouts.Any(e => e.Id == id);
        }
    }
}
