using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGameOrmianin {
    internal class SpellCard : Card {
        public SpellCard(string name, int damage, char element) : base("Spell Card", 50, 'a') {
            //ctor
        }
    }
}
