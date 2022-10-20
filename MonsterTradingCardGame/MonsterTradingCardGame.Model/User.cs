namespace MonsterTradingCardGame.Model {
    public class User {
        // ctor
        public User(string name, string password) {
            this.name = name;
            this.password = password;
        }
        
        // fields
        string name = "";
        string password = "";
        int coins = 20;
        Deck allCards = new Deck();
        Deck battleDeck = new Deck();

        // properties
        public string Name {
            get => name;
            set => name = value;
        }
        
        public string Password {
            get => password;
            set => password = value;
        }

        public static bool Login(string? username, string? password) {
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
        }
    }
}