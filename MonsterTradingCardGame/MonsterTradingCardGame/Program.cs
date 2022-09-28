using MonsterTradingCardGame.BL;
using MonsterTradingCardGame.Model;

namespace MonsterTradingCardGame {
    internal class Program {
        public static void Main(string[] args) {
            Console.WriteLine("Hello. Please login");

            var player1 = new User("Rudolf", "1234");
            var player2 = new User("Susanne", "anna999");
            var gameHandler = new GameHandler();
            //gameHandler.PerformBattle(player1);
        }
    }
}