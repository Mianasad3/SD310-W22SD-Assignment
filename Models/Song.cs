using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTunes.Models
{
    public partial class Song
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public int ArtistId { get; set; }
        [Required]
        public decimal? Price { get; set; }
        [NotMapped] public DateTime AddedOn { get; set; }
        [NotMapped] public int SoldCopies { get; set; }

        public virtual Artist Artist { get; set; } = null!;
    }
}
