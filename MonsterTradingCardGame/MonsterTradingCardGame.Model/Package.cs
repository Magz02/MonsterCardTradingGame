using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Model {
    public class Package {
        // ctor
        public Package() {
            // create a new package
            this.Cards = new List<Card>();
        }

        // fields
        public List<Card> Cards { get; set; }
    }
}
