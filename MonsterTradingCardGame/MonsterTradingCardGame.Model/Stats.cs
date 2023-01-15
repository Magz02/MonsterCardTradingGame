using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Model {
    public class Stats {
        // ctor
        public Stats(int games, int wins, int losses, int elo) {
            this.Games = games;
            this.Wins = wins;
            this.Losses = losses;
            this.Elo = elo;
        }

        // properties
        public int Games { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Elo { get; set; }
    }
}
