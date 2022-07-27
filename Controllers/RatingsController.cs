using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyTunes.Models;

namespace MyTunes.Controllers
{
    public class RatingsController : Controller
    {
        private readonly MyTunesContext _context;

        public RatingsController(MyTunesContext context)
        {
            _context = context;
        }

        // GET: Ratings
        public async Task<IActionResult> Index()
        {
              return _context.Ratings != null ? 
                          View(await _context.Ratings.ToListAsync()) :
                          Problem("Entity set 'MyTunesContext.Ratings'  is null.");
        }

        // GET: Ratings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Ratings == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // GET: Ratings/Create
        public IActionResult Create(int userId, int songId)
        {
            ViewData["Users"] = new SelectList(_context.Users, "Id", "Name");
            ViewData["Songs"] = new SelectList(_context.Songs, "Id", "Title");
            var rating = _context.Ratings.Where(x => x.UserId == userId && x.SongId == songId).FirstOrDefault();
            if (rating == null)
                rating = new Rating();
            return View(rating);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SongId,UserId,Rating1")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                if (rating.Id > 0)
                    _context.Update(rating);
                else
                    _context.Add(rating);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create), new { userId = rating.UserId, songId = rating.SongId});
            }
            ViewData["Users"] = new SelectList(_context.Users, "Id", "Name");
            ViewData["Songs"] = new SelectList(_context.Songs, "Id", "Title");
            return View(rating);
        }

        // GET: Ratings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ratings == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }
            return View(rating);
        }

        // POST: Ratings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SongId,UserId,Rating1")] Rating rating)
        {
            if (id != rating.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RatingExists(rating.Id))
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
            return View(rating);
        }

        // GET: Ratings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Ratings == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // POST: Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Ratings == null)
            {
                return Problem("Entity set 'MyTunesContext.Ratings'  is null.");
            }
            var rating = await _context.Ratings.FindAsync(id);
            if (rating != null)
            {
                _context.Ratings.Remove(rating);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RatingExists(int id)
        {
          return (_context.Ratings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
