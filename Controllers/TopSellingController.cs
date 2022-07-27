using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyTunes.Models;

namespace MyTunes.Controllers
{
    public class TotalRevenue
    {
        public decimal Revenue { get; set; }
        public int Songs { get; set; }
        public DateTime Date { get; set; }
    }

    public class TopSellingController : Controller
    {
        // Viewmodel for Topselling song / Topselling artist / Top 3 rated songs all in one
        TopSellingViewModel topSellingViewModel { get; set; }

        // Model to total revenue page
        public TotalRevenue totalRevenue { get; set; }
        private MyTunesContext _context; 
        public TopSellingController(MyTunesContext context)
        {
            _context = context;
        }
        // On page load get top sellings
        public IActionResult Index()
        {
            topSellingViewModel = new TopSellingViewModel() { TopRatedSongs = new Dictionary<string, decimal>(), TopSellingSong = new Song()};

            // In order to get Top selling song id, Group the user collections by songids and then got count of top group and number of group items
            var topSellingSongId = _context.UserSongs.GroupBy(x => x.SongId).Select(y => new { count = y.Count(), songid = y.FirstOrDefault().SongId }).OrderByDescending( z => z.count).FirstOrDefault();
            if (topSellingSongId != null && topSellingSongId.songid > 0)
            {
                // Based on top song id, get song
                topSellingViewModel.TopSellingSong = _context.Songs.Include(x => x.Artist).Where(x => x.Id == topSellingSongId.songid).FirstOrDefault();
                // After setting top sold song, set sold copies for that song
                topSellingViewModel.TopSellingSong.SoldCopies = topSellingSongId.count;
            }

            // Now it's time to get top rated songs

            // Got distinct ratings from ratings table based on songid
            List<Rating> distinctSongs = _context.Ratings.GroupBy(x => x.SongId).Select(x=>x.FirstOrDefault()).ToList();
            Dictionary<int, decimal> songRatings = new Dictionary<int, decimal>();
            // Loop through songids just got from ratings table and get commulative rating / total ratings to get average rating for each song
            foreach (var item in distinctSongs)
            {
                int totalRanting = _context.Ratings.Where(x => x.SongId == item.SongId).Sum(x => x.Rating1);
                int ratingCount = _context.Ratings.Where(x => x.SongId == item.SongId).Count();
                songRatings.Add(item.SongId, totalRanting / ratingCount);
            }
            List<KeyValuePair<int, decimal>> TopThreeRatedSongs = new List<KeyValuePair<int, decimal>>();

            // Decition if 3 or more than 3 songs are rated in the system or not.
            
            if (songRatings != null && songRatings.Count >= 3) // If 3 or more than 3 song are rated then get top 3 songs
                TopThreeRatedSongs = songRatings.OrderByDescending(x => x.Value).Take(3).ToList();
            else // If less than 3 songs are rated then get all rated songs
                TopThreeRatedSongs = songRatings.OrderByDescending(x => x.Value).ToList();

            // Add songs details based on song ids picked above
            if(TopThreeRatedSongs != null)
                foreach (var item in TopThreeRatedSongs)
                    topSellingViewModel.TopRatedSongs.Add(_context.Songs.Where(x => x.Id == item.Key).FirstOrDefault().Title, item.Value);
            return View(topSellingViewModel);
        }

        public IActionResult TotalRevenue()
        {
            return View();
        }
            [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TotalRevenue([Bind("Date")] TotalRevenue revenue)
        {
            totalRevenue = new TotalRevenue();

            // Get song ids for selected month and year
            List<int> songIds = _context.UserSongs.Where(x=>x.AddedOn.Value.Year == revenue.Date.Year &&
            x.AddedOn.Value.Month == revenue.Date.Month).Select(x => x.SongId).ToList();
            if(songIds != null && songIds.Count > 0)
            {
                // Total amount earned against the selected songs
                decimal totalAmt = _context.Songs.Where(x => songIds.Contains(x.Id)).Select(x => x.Price).Sum().Value;
                int totalSongs = songIds.Count();
                totalRevenue.Revenue = totalAmt;
                totalRevenue.Songs = totalSongs;

                // Indicator to show revenue details on front end
                ViewBag.RevenueCalculated = true;
            }
            totalRevenue.Date = revenue.Date;
            return View(totalRevenue);
        }
    }
}
