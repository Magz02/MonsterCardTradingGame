namespace MonsterTradingCardGame.Model {
    public class User {
        //fields
        string name = "";
        string password = "";

        //ctor
        public User(string name, string password) {
            this.name = name;
            this.password = password;
        }
    }
}