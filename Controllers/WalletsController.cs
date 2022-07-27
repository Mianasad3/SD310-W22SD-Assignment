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
    public class WalletsController : Controller
    {
        private readonly MyTunesContext _context; 
        public WalletsController(MyTunesContext context)
        {
            _context = context;
        }

        // GET: Wallets
        public async Task<IActionResult> Index()
        {
            ViewData["Users"] = new SelectList(_context.Users, "Id", "Name"); 
            return View();
        }
        [HttpPost]
        public IActionResult GetUserWallet([Bind("Id,UserId")] Wallet wallet)
        {
            int userId = wallet.UserId;
            wallet = _context.Wallets.Where(x => x.UserId == userId).FirstOrDefault();
            if (wallet == null)
                ViewBag.ErrorMessage = "No wallet found.";
            ViewData["Users"] = new SelectList(_context.Users, "Id", "Name");
            return View("Index", wallet);            
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Users"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Balance")] Wallet wallet)
        {
            try
            {
                ModelState.Remove("User");
                ModelState.Remove("Id");
                if (ModelState.IsValid)
                {
                    _context.Add(wallet);
                    await _context.SaveChangesAsync();
                    ViewBag.SuccessMessage = "Wallet Created";
                }
                else
                {
                    ViewBag.ErrorMessage = "Form incomplete";
                }
                ViewData["Users"] = new SelectList(_context.Users, "Id", "Name");
                return View(wallet);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Wallet create failed";
                return View(wallet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([Bind("Id,UserId,Balance")] Wallet wallet)
        {
            try
            {
                ModelState.Remove("User"); 
                if (ModelState.IsValid)
                {
                    _context.Update(wallet);
                    await _context.SaveChangesAsync();
                    ViewBag.SuccessMessage = "Wallet Updated";
                }
                else
                {
                    ViewBag.ErrorMessage = "Form incomplete";
                }
                ViewData["UserId"] = new SelectList(_context.Users, "Id", "Name", wallet.UserId);
                return View("Index", wallet);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Wallet Update failed";
                return View(wallet);
            }
        }

        // GET: Wallets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Wallets == null)
            {
                return NotFound();
            }

            var wallet = await _context.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wallet == null)
            {
                return NotFound();
            }

            return View(wallet);
        }

        // POST: Wallets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Wallets == null)
            {
                return Problem("Entity set 'MyTunesContext.Wallets'  is null.");
            }
            var wallet = await _context.Wallets.FindAsync(id);
            if (wallet != null)
            {
                _context.Wallets.Remove(wallet);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WalletExists(int id)
        {
          return (_context.Wallets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
