using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using FitApp.Models;

namespace FitApp.Controllers
{
    public class UserSpecificWorkoutsUnitTestController : Controller
    {
        // Use the in-memory data from WorkoutsUnitTestController
        private static List<UserSpecificWorkout> userSpecificWorkouts = WorkoutsUnitTestController.userSpecificWorkouts;

        // GET: UserSpecificWorkoutsUnitTest
        public IActionResult Index()
        {
            return View(userSpecificWorkouts);
        }

        // GET: UserSpecificWorkoutsUnitTest/Details/5
        public IActionResult Details(int id)
        {
            var userWorkout = userSpecificWorkouts.Find(w => w.Id == id);
            if (userWorkout == null)
            {
                return NotFound();
            }
            return View(userWorkout);
        }

        // GET: UserSpecificWorkoutsUnitTest/Delete/5
        public IActionResult Delete(int id)
        {
            var userWorkout = userSpecificWorkouts.Find(w => w.Id == id);
            if (userWorkout == null)
            {
                return NotFound();
            }
            return View(userWorkout);
        }

        // POST: UserSpecificWorkoutsUnitTest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var userWorkout = userSpecificWorkouts.Find(w => w.Id == id);
            if (userWorkout != null)
            {
                userSpecificWorkouts.Remove(userWorkout);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
