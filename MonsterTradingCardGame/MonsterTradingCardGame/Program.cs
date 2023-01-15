using MonsterTradingCardGame.BL;
using MonsterTradingCardGame.Model;

namespace MonsterTradingCardGame {
    public class Program {
        public static void Main(string[] args) {
            GameHandler handler = new GameHandler();
            handler.run();
        }
    }
}