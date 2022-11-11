namespace MonsterTradingCardGame.Model {
    public class User {
        public User(string username, string password) {
            this.Username = username;
            this.Password = password;
        }
       
        // fields
        int coins = 20;
        Deck allCards = new Deck();
        Deck battleDeck = new Deck();

        // properties
        public string Username { get; set; }
        public string Password { get; set; }

        /*public static bool Login(string? username, string? password) {
            //throw new NotImplementedException();

            if (username == "Test" && password == "1234") {
                return true;
            } else {
                return false;
            }
        }

        public static bool Register(string username, string password) {
            throw new NotImplementedException();
        }

        public static User LoginSuccess(string username, string password) {
            return new User(username, password);
        }*/
    }
}