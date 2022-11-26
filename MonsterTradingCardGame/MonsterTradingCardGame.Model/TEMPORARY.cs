using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Model {
    public class TEMPORARY {
        public TEMPORARY(User user) {
            this.User = user;
        }

        public User User { get; }

        public void Announce() {
            Console.WriteLine($"Announcing new login: User {User.Username} logged in with token {User.Token}!");
        }
    }
}
