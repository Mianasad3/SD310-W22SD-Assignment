namespace MyTunes.Models
{
    public class TopSellingViewModel
    {
        public Song TopSellingSong { get; set; }
        public Dictionary<string, decimal> TopRatedSongs { get; set; }
    }
}
