using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using FitApp.Models;

namespace FitApp.Controllers
{
    public class WorkoutsUnitTestController : Controller
    {
        // Sample data to simulate in-memory data for unit tests
        private static List<Workouts> testWorkouts = new List<Workouts>
        {
            new Workouts { Id = 222, Name = "Sample Workout 1", Description = "Description 1", CaloriesBurned = 200, Duration = new System.TimeSpan(0, 30, 0) },
            new Workouts { Id = 223, Name = "Sample Workout 2", Description = "Description 2", CaloriesBurned = 300, Duration = new System.TimeSpan(0, 45, 0) }
        };
        // Static list to store the in-memory user-specific saved workouts
        public static List<UserSpecificWorkout> userSpecificWorkouts = new List<UserSpecificWorkout>();

        // GET: WorkoutsUnitTest
        public IActionResult Index()
        {
            return View(testWorkouts);
        }

        // GET: WorkoutsUnitTest/Details/5
        public IActionResult Details(int id)
        {
            var workout = testWorkouts.Find(w => w.Id == id);
            if (workout == null)
            {
                return NotFound();
            }
            return View(workout);
        }

        // GET: WorkoutsUnitTest/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkoutsUnitTest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Workouts workout)
        {
            if (ModelState.IsValid)
            {
                workout.Id = testWorkouts.Count + 1;  // Simulate ID increment
                testWorkouts.Add(workout);
                return RedirectToAction(nameof(Index));
            }
            return View(workout);
        }

        // GET: WorkoutsUnitTest/Edit/5
        public IActionResult Edit(int id)
        {
            var workout = testWorkouts.Find(w => w.Id == id);
            if (workout == null)
            {
                return NotFound();
            }
            return View(workout);
        }

        // POST: WorkoutsUnitTest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Workouts workout)
        {
            var existingWorkout = testWorkouts.Find(w => w.Id == id);
            if (existingWorkout == null)
            {
                return NotFound();
            }

            existingWorkout.Name = workout.Name;
            existingWorkout.Description = workout.Description;
            existingWorkout.CaloriesBurned = workout.CaloriesBurned;
            existingWorkout.Duration = workout.Duration;

            return RedirectToAction(nameof(Index));
        }

        // GET: WorkoutsUnitTest/Delete/5
        public IActionResult Delete(int id)
        {
            var workout = testWorkouts.Find(w => w.Id == id);
            if (workout == null)
            {
                return NotFound();
            }
            return View(workout);
        }

        // POST: WorkoutsUnitTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var workout = testWorkouts.Find(w => w.Id == id);
            if (workout != null)
            {
                testWorkouts.Remove(workout);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Save a workout to UserSpecificWorkouts
        public IActionResult SaveWorkout(int id)
        {
            var workout = testWorkouts.Find(w => w.Id == id);
            if (workout == null)
            {
                return NotFound();
            }

            var userWorkout = new UserSpecificWorkout
            {
                Id = userSpecificWorkouts.Count + 1,  // Simulate ID increment
                Name = workout.Name,
                Description = workout.Description,
                CaloriesBurned = workout.CaloriesBurned,
                Duration = workout.Duration
            };

            userSpecificWorkouts.Add(userWorkout);
            return RedirectToAction(nameof(Index));  // Redirect to WorkoutsUnitTest index after saving
        }
    }
}
