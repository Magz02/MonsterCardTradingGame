using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGameOrmianin {
    internal class User {
        // fields
        private string _username = "";
        private string _password = "";
        private int _coins = 20;
        private List<Card>? allCards;
        private List<Card>? bestCards;

        // ctor
        public User(string username) {
            this._username = username;
        }

        // methods
        public void ManageCards(List<Card> cards) {
            // here goes code
        }

        public List<Card> Battle(List<Card> bestCards) {
            return bestCards;
        }
    }
}
