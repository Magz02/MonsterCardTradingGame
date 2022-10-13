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
        Deck deck = new Deck();

        // properties
        public string Name {
            get => name;
            set => name = value;
        }
        
        public string Password {
            get => password;
            set => password = value;
        }
    }
}