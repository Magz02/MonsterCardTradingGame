using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Model {
    public class Package {
        // ctor
        public Package(int id, string card1id, string card2id, string card3id, string card4id, string card5id) {
            this.Id = id;
            this.Card1id = card1id;
            this.Card2id = card2id;
            this.Card3id = card3id;
            this.Card4id = card4id;
            this.Card5id = card5id;
        }


        // fields
        public int Id { get; set; }
        public string Card1id { get; set; }
        public string Card2id { get; set; }
        public string Card3id { get; set; }
        public string Card4id { get; set; }
        public string Card5id { get; set; }

    }
}
