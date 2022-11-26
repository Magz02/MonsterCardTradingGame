using MonsterTradingCardGame.Model;
using MonsterTradingCardGame.BL.HTTP;

namespace MonsterTradingCardGame.BL {
    public class GameHandler {
        HttpServer server = new HttpServer(System.Net.IPAddress.Any, 10001);
        public GameHandler() {
            //ctor
        }

        /*public static void PerformBattle(User player) {
            Console.WriteLine($"{player}");
        }*/

        public void run() {
            server.Run();
        }
    }
}