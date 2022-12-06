using MonsterTradingCardGame.BL;
using MonsterTradingCardGame.Model;

namespace MonsterTradingCardGame {
    public class Program {
        public static void Main(string[] args) {
            GameHandler handler = new GameHandler();
            handler.run();












            /*GameHandler handler = new GameHandler();
            Thread handlerThread = new Thread(handler.run);
            handlerThread.Start();
            User currentUser;
            char choice;
            Console.WriteLine("Hello. Please login or register");

            bool success = false;

            do {
                choice = Console.ReadLine()[0];
                switch (choice) {
                    case 'l':
                        Console.WriteLine("Please enter your username");
                        string username = Console.ReadLine();
                        Console.WriteLine("Please enter your password");
                        string password = Console.ReadLine();
                        success = User.Login(username, password);
                        if (success) {
                            Console.WriteLine("Login successful");
                            currentUser = User.LoginSuccess(username, password);
                            //TODO: Handler login anstatt login in User
                            Console.WriteLine($"Welcome {currentUser.Username}!");
                            success = true;
                        } else {
                            Console.WriteLine("Login failed ,try again");
                            Console.WriteLine("Hello. Please login or register");
                        }
                        break;

                    case 'r':
                        Console.WriteLine("Please enter your username");
                        username = Console.ReadLine();
                        Console.WriteLine("Please enter your password");
                        password = Console.ReadLine();
                        success = User.Register(username, password);
                        if (success) {
                            Console.WriteLine("Registration successful");
                        } else {
                            Console.WriteLine("Registration failed");
                            Console.WriteLine("Try again");
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid choice, try again");
                        choice = Console.ReadLine()[0];
                        break;
                }
            } while (!success);

            // var player1 = new User("Rudolf", "1234");
            // var player2 = new User("Susanne", "anna999");
            // var gameHandler = new GameHandler();
            //gameHandler.PerformBattle(player1);*/
        }
    }
}