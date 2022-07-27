using System;
using System.Collections.Generic;

namespace MyTunes.Models
{
    public partial class UserSong
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SongId { get; set; }
        public DateTime? AddedOn { get; set; }
    }
}
