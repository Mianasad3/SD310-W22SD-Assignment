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
    public class UserCollectionController : Controller
    {
        private readonly MyTunesContext _context;

        public UserCollectionController(MyTunesContext context)
        {
            _context = context;
        }

        // GET: Songs
        public async Task<IActionResult> Index()
        {
            ViewData["Users"] = new SelectList(_context.Users, "Id", "Name");
            return View();
        }
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilterCollection([Bind("UserId")] UserSong song)
        {
            List<Song> songs = new List<Song>();
            if (song.UserId > 0)
            {                
                var userSongs = _context.UserSongs.Where(x => x.UserId == song.UserId).ToList();
                foreach (var item in userSongs)
                {
                    var currSong = _context.Songs.Where(x => x.Id == item.SongId).FirstOrDefault();
                    currSong.AddedOn = item.AddedOn.Value;
                    songs.Add(currSong);
                }
                    
            }
            ViewBag.UserId = song.UserId;
            return View("UserSongs", songs);
        } 

        public IActionResult Return(int userId, int songId)
        {
            // remove song from user collection based on song id
            _context.UserSongs.Remove(_context.UserSongs.Where(x => x.UserId == userId && x.SongId == songId).FirstOrDefault());
            var userWallet = _context.Wallets.Where(x => x.UserId == userId).FirstOrDefault();
            if(userWallet != null)
            {
                // Return song amount to user wallet
                userWallet.Balance = userWallet.Balance + _context.Songs.Where(x => x.Id == songId).FirstOrDefault().Price.Value;
                _context.Wallets.Update(userWallet);
            }
            _context.SaveChanges();
            List<Song> songs = new List<Song>();
            if (userId > 0)
            {
                // Read updated records for user collection before returning view
                var userSongs = _context.UserSongs.Where(x => x.UserId == userId).ToList();
                foreach (var item in userSongs)
                {
                    var currSong = _context.Songs.Where(x => x.Id == item.SongId).FirstOrDefault();
                    currSong.AddedOn = item.AddedOn.Value;
                    songs.Add(currSong);
                }
            }
            ViewBag.UserId = userId;
            return View("UserSongs", songs);
        }
         
        public IActionResult Create()
        {
            ViewData["Users"] = new SelectList(_context.Users, "Id", "Name");  
            return View();
        }

        public JsonResult GetSongs(int userId)
        {
            return new JsonResult(GetUserSongs(userId));
        }
        // POST: Songs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,SongId")] UserSong userSong)
        {
            ModelState.Remove("Id");
            if (ModelState.IsValid)
            {
                // Get use wallet
                Wallet userWallet = _context.Wallets.Where(x => x.UserId == userSong.UserId).FirstOrDefault();
                // Get song
                Song song = _context.Songs.Where(x => x.Id == userSong.SongId).FirstOrDefault();

                // If no wallet found, return view as error message: No wallet found
                if (userWallet == null)
                {
                    ViewBag.ErrorMessage = "No user wallet found. Please create a new wallet";
                    ViewData["Users"] = new SelectList(_context.Users, "Id", "Name");
                   
                    ViewData["Songs"] = new MultiSelectList(GetUserSongs(userSong.UserId), "Id", "Title");
                    return View();
                }
                // If wallet balance less than song price, return view as error message: insufficient balance
                if (userWallet.Balance < song.Price)
                {
                    ViewBag.ErrorMessage = "Insufficient balance. Please recharge your wallet.";
                    ViewData["Users"] = new SelectList(_context.Users, "Id", "Name");
                    ViewData["Songs"] = new MultiSelectList(GetUserSongs(userSong.UserId), "Id", "Title");
                    return View();
                }
                // Set today date as addedon to the user collection record
                userSong.AddedOn = DateTime.Today;
                _context.Add(userSong);
               
                // Deduct song price from user wallet
                if (userWallet != null)
                {
                    userWallet.Balance = userWallet.Balance - _context.Songs.Where(x => x.Id == userSong.SongId).FirstOrDefault().Price.Value;
                    _context.Wallets.Update(userWallet);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Users"] = new SelectList(_context.Users, "Id", "Name");
            ViewData["Songs"] = new MultiSelectList(GetUserSongs(userSong.UserId), "Id", "Title");
            return View();
        }

        // Private method to get songs against user id
        private List<Song> GetUserSongs(int userId)
        {
            List<int> userSongIds = _context.UserSongs.Where(x => x.UserId == userId).Select(x => x.SongId).ToList();
            List<Song> songs = new List<Song>();
            if (userSongIds != null && userSongIds.Count > 0)
            {
                songs = _context.Songs.Where(x => !userSongIds.Contains(x.Id)).ToList();

            }
            else
            {
                songs = _context.Songs.ToList(); 
            }
            return songs;
        }

    }
}
