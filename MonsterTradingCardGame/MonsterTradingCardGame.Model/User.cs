namespace MonsterTradingCardGame.Model {
    public class User {
        public User(string username, string password) {
            this.Username = username;
            this.Password = password;
        }
       
        // fields
        List<Card> allCards;
        List<Card> deck;
        bool isLoggedIn = false;

        // properties
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public int Elo { get; set; }
        public int Coins { get; set; }
        public int Id { get; set; }
    }
}