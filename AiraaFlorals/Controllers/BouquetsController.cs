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
    public class BouquetsController : Controller
    {
        private readonly FloralsContext _context;

        public BouquetsController(FloralsContext context)
        {
            _context = context;
        }

        // GET: Bouquets
        public async Task<IActionResult> Index()
        {
            var floralsContext = _context.Bouquets.Include(b => b.Occasion);
            return View(await floralsContext.ToListAsync());
        }

        // GET: Bouquets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bouquets == null)
            {
                return NotFound();
            }

            var bouquets = await _context.Bouquets
                .Include(b => b.Occasion)
                .FirstOrDefaultAsync(m => m.BouquetId == id);
            if (bouquets == null)
            {
                return NotFound();
            }

            return View(bouquets);
        }

        // GET: Bouquets/Create
        public IActionResult Create()
        {
            ViewData["OccasionId"] = new SelectList(_context.Occasions, "OccasionId", "OccasionName");
            return View();
        }

        // POST: Bouquets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BouquetId,Name,Image,Description,Price,Stock,OccasionId")] Bouquets bouquets, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null && Image.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await Image.CopyToAsync(stream);
                        bouquets.Image = stream.ToArray();
                    }
                }

                _context.Add(bouquets);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OccasionId"] = new SelectList(_context.Occasions, "OccasionId", "OccasionName", bouquets.OccasionId);
            return View(bouquets);
        }

        // GET: Bouquets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Bouquets == null)
            {
                return NotFound();
            }

            var bouquets = await _context.Bouquets.FindAsync(id);
            if (bouquets == null)
            {
                return NotFound();
            }
            ViewData["OccasionId"] = new SelectList(_context.Occasions, "OccasionId", "OccasionName", bouquets.OccasionId);
            return View(bouquets);
        }

        // POST: Bouquets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BouquetId,Name,Image,Description,Price,Stock,OccasionId")] Bouquets bouquets)
        {
            if (id != bouquets.BouquetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bouquets);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BouquetsExists(bouquets.BouquetId))
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
            ViewData["OccasionId"] = new SelectList(_context.Occasions, "OccasionId", "OccasionName", bouquets.OccasionId);
            return View(bouquets);
        }

        // GET: Bouquets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bouquets == null)
            {
                return NotFound();
            }

            var bouquets = await _context.Bouquets
                .Include(b => b.Occasion)
                .FirstOrDefaultAsync(m => m.BouquetId == id);
            if (bouquets == null)
            {
                return NotFound();
            }

            return View(bouquets);
        }

        // POST: Bouquets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bouquets == null)
            {
                return Problem("Entity set 'FloralsContext.Bouquets'  is null.");
            }
            var bouquets = await _context.Bouquets.FindAsync(id);
            if (bouquets != null)
            {
                _context.Bouquets.Remove(bouquets);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BouquetsExists(int id)
        {
          return (_context.Bouquets?.Any(e => e.BouquetId == id)).GetValueOrDefault();
        }
    }
}
