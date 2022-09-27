using MonsterTradingCardsGame.BL;
using MonsterTradingCardsGame.Model;

namespace MonsterTradingCardsGame {
    internal class Program {
       public static void Main(string[] args) {
            Console.WriteLine("Hello, please login");

            var player1 = new User("Rudolf");
            var player2 = new User("Susanne");
            var gameHandler = new GameHandler();
            gameHandler.PerformBattle(player1);
        }
    }
}