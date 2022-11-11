using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Model.Enums;

namespace MonsterTradingCardGame.Model {
    public class Card {
        // ctor
        public Card(string name, Enums.Type type, Element element, int dmg) {
            this.Name = name;
            this.Type = type;
            this.Element = element;
            this.dmg = dmg;
        }

        // fields
        int dmg;

        // properties
        public string Name { get; }

        public Enums.Type Type { get; }
        public Element Element { get; }
    }
}
