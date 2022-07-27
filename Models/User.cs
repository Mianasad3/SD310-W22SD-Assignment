using System;
using System.Collections.Generic;

namespace MyTunes.Models
{
    public partial class User
    {
        public User()
        {
            Wallets = new HashSet<Wallet>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Wallet> Wallets { get; set; }
    }
}
