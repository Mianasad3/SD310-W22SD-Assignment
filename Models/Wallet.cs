using System;
using System.Collections.Generic;

namespace MyTunes.Models
{
    public partial class Wallet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
