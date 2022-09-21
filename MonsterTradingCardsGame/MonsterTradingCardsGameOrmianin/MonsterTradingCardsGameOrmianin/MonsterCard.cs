using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardsGameOrmianin {
    internal class MonsterCard : Card {
        public MonsterCard(string name, int damage, char element) : base("Monster Card", 20, 'e') {
            //ctor
        }
    }
}
