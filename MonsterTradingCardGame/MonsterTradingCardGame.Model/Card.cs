using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame.Model.Enums;

namespace MonsterTradingCardGame.Model {
    public class Card {
        // ctor
        public Card(string id, string name, string element, string type, double damage) {
            this.Id = id;
            this.Name = name;
            this.Element = (Element)Enum.Parse(typeof(Element), element);
            this.Type = (Enums.Type)Enum.Parse(typeof(Enums.Type), type);
            this.Damage = damage;
        }
        
        // properties
        public double Damage { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }

        public Enums.Type Type { get; set; }
        public Element Element { get; set; }

        public bool Chosen { get; set; }
        
        public string Owner { get; set; }
    }
}
