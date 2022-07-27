using System;
using System.Collections.Generic;

namespace MyTunes.Models
{
    public partial class Rating
    {
        public int Id { get; set; }
        public int SongId { get; set; }
        public int UserId { get; set; }
        public int Rating1 { get; set; }
    }
}
