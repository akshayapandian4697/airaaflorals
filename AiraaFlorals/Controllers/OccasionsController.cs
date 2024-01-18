using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AiraaFlorals.Models;

namespace AiraaFlorals.Controllers
{
    public class OccasionsController : Controller
    {
        private readonly FloralsContext _context;

        public OccasionsController(FloralsContext context)
        {
            _context = context;
        }

        // GET: Occasions
        public async Task<IActionResult> Index()
        {
              return _context.Occasions != null ? 
                          View(await _context.Occasions.ToListAsync()) :
                          Problem("Entity set 'FloralsContext.Occasions'  is null.");
        }

        // GET: Occasions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Occasions == null)
            {
                return NotFound();
            }

            var occasion = await _context.Occasions
                .FirstOrDefaultAsync(m => m.OccasionId == id);
            if (occasion == null)
            {
                return NotFound();
            }

            return View(occasion);
        }

        // GET: Occasions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Occasions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OccasionId,OccasionName")] Occasion occasion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(occasion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(occasion);
        }

        // GET: Occasions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Occasions == null)
            {
                return NotFound();
            }

            var occasion = await _context.Occasions.FindAsync(id);
            if (occasion == null)
            {
                return NotFound();
            }
            return View(occasion);
        }

        // POST: Occasions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OccasionId,OccasionName")] Occasion occasion)
        {
            if (id != occasion.OccasionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(occasion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OccasionExists(occasion.OccasionId))
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
            return View(occasion);
        }

        // GET: Occasions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Occasions == null)
            {
                return NotFound();
            }

            var occasion = await _context.Occasions
                .FirstOrDefaultAsync(m => m.OccasionId == id);
            if (occasion == null)
            {
                return NotFound();
            }

            return View(occasion);
        }

        // POST: Occasions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Occasions == null)
            {
                return Problem("Entity set 'FloralsContext.Occasions'  is null.");
            }
            var occasion = await _context.Occasions.FindAsync(id);
            if (occasion != null)
            {
                _context.Occasions.Remove(occasion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OccasionExists(int id)
        {
          return (_context.Occasions?.Any(e => e.OccasionId == id)).GetValueOrDefault();
        }
    }
}
