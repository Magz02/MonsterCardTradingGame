using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Model {
    public class Card {
        // ctor
        public Card(String name, int type, int element, int dmg) {
            this.name = name;
            this.type = type;
            this.element = element;
            this.dmg = dmg;
        }

        // fields
        String name;
        int type; // 0 = Monster, 1 = Spell
        int element; // 0 = Neutral, 1 = Fire, 2 = Water
        int dmg; 

        // properties
        public String Name {
            get { return this.name; }
        }

        public int Type {
            get { return this.type; }
        }
    }
}
