using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitApp.Data;

namespace FitApp.Models
{
    public class WorkoutsController : Controller
    {
        private readonly FitAppContext _context;

        public WorkoutsController(FitAppContext context)
        {
            _context = context;
        }

        // GET: Workouts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Workouts.ToListAsync());
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Workouts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workouts = await _context.Workouts.FindAsync(id);
            if (workouts != null)
            {
                _context.Workouts.Remove(workouts);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutsExists(int id)
        {
            return _context.Workouts.Any(e => e.Id == id);
        }
    }
}
