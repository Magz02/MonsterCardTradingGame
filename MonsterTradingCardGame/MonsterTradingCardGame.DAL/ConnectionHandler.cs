using MonsterTradingCardGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.DAL {
    public class ConnectionHandler {
        
        User currentUser;

        public ConnectionHandler(User currentUser) {
            this.currentUser = currentUser;
        }
        
    }
}
