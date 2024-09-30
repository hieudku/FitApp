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

        // GET: UserSpecificWorkouts
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var userWorkouts = _context.UserSpecificWorkouts
                .Where(uw => uw.UserId == userId);

            return View(await userWorkouts.ToListAsync());
        }

        // GET: UserSpecificWorkouts/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: UserSpecificWorkouts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserSpecificWorkouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Duration,CaloriesBurned")] UserSpecificWorkout userWorkout)
        {
            if (ModelState.IsValid)
            {
                userWorkout.UserId = _userManager.GetUserId(User);
                _context.UserSpecificWorkouts.Add(userWorkout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View(userWorkout);
        }

        // GET: UserSpecificWorkouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            return View(userWorkout);
        }

        // POST: UserSpecificWorkouts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Duration,CaloriesBurned")] UserSpecificWorkout userWorkout)
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
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View(userWorkout);
        }

        // GET: UserSpecificWorkouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: UserSpecificWorkouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
            return RedirectToAction(nameof(Index));
        }

        private bool UserSpecificWorkoutExists(int id)
        {
            return _context.UserSpecificWorkouts.Any(e => e.Id == id);
        }
    }
}
