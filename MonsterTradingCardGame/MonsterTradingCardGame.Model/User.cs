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
        bool isLoggedIn = false;

        // properties
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        
    }
}