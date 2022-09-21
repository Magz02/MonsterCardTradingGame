using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGameOrmianin {
    internal class Card {
        // fields
        private string _name = "";
        private readonly int _damage;
        private char _element = 'd';

        // ctor
        public Card(string name, int damage, char element) {
            this._name = name;
            this._damage = damage;
            this._element = element;
        }
    }
}