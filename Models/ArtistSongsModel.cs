using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MyTunes.Models
{
    public class ArtistSongsModel
    {
        public Artist? Artist { get; set; }
        public List<Song>? Songs {get;set;}
    }
}