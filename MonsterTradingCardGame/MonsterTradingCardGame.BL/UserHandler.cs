using MonsterTradingCardGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL {
    public class UserHandler {
        private User User { get; set; }

        public UserHandler(User user) {
            this.User = user;
        }
    }
}
