using MonsterTradingCardGame.Model;
using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model.Enums;
using System.Data;
using Npgsql;
using MonsterTradingCardGame.BL;
using MonsterTradingCardGame.Model.Logger;

namespace MonsterTradingCardGame.Test {
    public class UnitTests {
        [SetUp]
        public void Setup() {
        }

        // TEST 1
        // tests whether User object is created properly
        [Test]
        public void TestUserCreation() {
            // Arrange
            User user = new User("TestUser", "TestPassword");
            Hash hash = new Hash();

            // Act
            string username = "TestUser";
            string password = "TestPassword";

            // Assert
            Assert.AreEqual(user.Username, username);
            Assert.AreEqual(hash.HashValue(user.Password), hash.HashValue(password));
        }

        // TEST 2
        // tests whether Card is created properly
        [Test]
        public void TestCardCreation() {
            // Arrange
            Card card = new Card("tid", "TestName", "Fire", "Monster", 666.0);

            // Act
            string id = "tid";
            string name = "TestName";
            Element element = (Element)Enum.Parse(typeof(Element), "Fire");
            Model.Enums.Type type = (Model.Enums.Type)Enum.Parse(typeof(Model.Enums.Type), "Monster");
            double damage = 666.0;

            // Assert
            Assert.AreEqual(card.Id, id);
            Assert.AreEqual(card.Name, name);
            Assert.That(card.Element == element);
            Assert.That(card.Type == type);
            Assert.AreEqual(card.Damage, damage);
        }

        // TEST 3
        // tests if a package is created properly
        [Test]
        public void TestPackageCreation() {
            // Arrange
            Package package = new Package(1, "id1", "id2", "id3", "id4", "id5");

            // Act
            int id = 1;
            string id1 = "id1";
            string id2 = "id2";
            string id3 = "id3";
            string id4 = "id4";
            string id5 = "id5";

            // Assert
            Assert.AreEqual(package.Id, id);
            Assert.AreEqual(package.Card1id, id1);
            Assert.AreEqual(package.Card2id, id2);
            Assert.AreEqual(package.Card3id, id3);
            Assert.AreEqual(package.Card4id, id4);
            Assert.AreEqual(package.Card5id, id5);
        }

        // TEST 4
        // Tests for user token if the user is logged in - existing user
        [Test]
        public void TestLoginValidatorAuthorizedCheck() {
            // Arrange
            IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
            connection.Open();
            LoggedInValidator validator = new LoggedInValidator();
            Dictionary<string, string> headers = new();
            User user = new User("TestU", "TestP");

            // Act
            headers.Add("Authorization", $"Basic {user.Username}-mtcgToken");
            bool result = validator.Validate(headers, connection);
            connection.Close();

            // Assert
            Assert.That(result == true);
        }

        // TEST 5
        // Tests for user token if the user is logged in - non existing user (should be false)
        [Test]
        public void TestLoginValidatorNonExistingUser() {
            // Arrange
            IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
            connection.Open();
            LoggedInValidator validator = new LoggedInValidator();
            Dictionary<string, string> headers = new();
            User user = new User("Doesntexist", "whatever");

            // Act
            headers.Add("Authorization", $"Basic {user.Username}-mtcgToken");
            bool result = validator.Validate(headers, connection);
            connection.Close();

            // Assert
            Assert.That(result == false);
        }

        // TEST 6
        // Tests for user token if the user exists but is not logged in - should return false
        [Test]
        public void TestLoginValidatorAuthorizedButNotLoggedInCheck() {
            // Arrange
            IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
            connection.Open();
            LoggedInValidator validator = new LoggedInValidator();
            Dictionary<string, string> headers = new();
            User user = new User("TestFU", "TestFP");

            // Act
            headers.Add("Authorization", $"Basic {user.Username}-mtcgToken");
            bool result = validator.Validate(headers, connection);
            connection.Close();

            // Assert
            Assert.That(result == false);
        }

        // TEST 7
        // Tries to validate but has no validation token - should return false
        [Test]
        public void TestLoginValidatorNotAuthorized() {
            // Arrange
            IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
            connection.Open();
            LoggedInValidator validator = new LoggedInValidator();
            Dictionary<string, string> headers = new();

            // Act
            bool result = validator.Validate(headers, connection);
            connection.Close();

            // Assert
            Assert.That(result == false);
        }

        // TEST 8
        // Validates whether user is admin
        [Test]
        public void TestLoginValidatorIsAdmin() {
            // Arrange
            IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
            connection.Open();
            LoggedInValidator validator = new LoggedInValidator();
            Dictionary<string, string> headers = new();
            User user = new User("admin", "istrator");

            // Act
            headers.Add("Authorization", $"Basic {user.Username}-mtcgToken");
            bool result = validator.ValidateAdmin(headers, connection);
            connection.Close();

            // Assert
            Assert.That(result == true);
        }

        // TEST 9
        // Validates whether user is admin but gives token of a normal user - should fail
        [Test]
        public void TestLoginValidatorIsNotAdmin() {
            // Arrange
            IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
            connection.Open();
            LoggedInValidator validator = new LoggedInValidator();
            Dictionary<string, string> headers = new();
            User user = new User("kienboec", "daniel");
            
            // Act
            headers.Add("Authorization", $"Basic {user.Username}-mtcgToken");
            bool result = validator.ValidateAdmin(headers, connection);
            connection.Close();

            // Assert
            Assert.That(result == false);
        }

        // TEST 10
        // Checks if enums are parsed correctly
        [Test]
        public void TestEnumParsing() {
            // Arrange
            string element = "Water";
            string type = "Monster";

            // Act
            Model.Enums.Element elementEnum = (Model.Enums.Element)Enum.Parse(typeof(Model.Enums.Element), element);
            Model.Enums.Type typeEnum = (Model.Enums.Type)Enum.Parse(typeof(Model.Enums.Type), type);

            // Assert
            Assert.That(elementEnum == Model.Enums.Element.Water);
            Assert.That(typeEnum == Model.Enums.Type.Monster);
        }

        // TEST 11
        // Checks if passwords are hashed correctly
        [Test]
        public void TestPasswordHashing() {
            // Arrange
            string password = "password";
            Hash hash = new Hash();

            // Act
            string hashedByHash = hash.HashValue(password);
            string hashedSHA = "XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=";

            // Assert
            Assert.That(hashedByHash == hashedSHA);
        }

        // TEST 12
        // Check stats class
        [Test]
        public void TestStats() {
            // Arrange
            Stats stats = new Stats(1, 2, 3, 100);

            // Act
            int games = stats.Games;
            int wins = stats.Wins;
            int losses = stats.Losses;
            int elo = stats.Elo;

            // Assert
            Assert.That(games == 1);
            Assert.That(wins == 2);
            Assert.That(losses == 3);
            Assert.That(elo == 100);
        }

        // TEST 13
        // Tests hashing under extremely long passwords
        [Test]
        public void TestLongPasswordHashing() {
            // Arrange
            Hash hash = new Hash();
            string password = "uhlghguoiagmcgicwoitewnyotevny8oqtvyn9tvunqewtv9ynpqewtvyn8oet8ynoqvvny8opvewqtvtynp" +
                "vewqtpynteqwvyn9pqewtvmicfuigf9yr7iuuuiuhgiynoicgviugoigolgvoingvuopvtumoiwevyefggjaijgoijgaijgasoigas" +
                "joigjoigadsjgdasiojjgdsaoijdgoijadsgoiadkgfklgsklgdasklgdasklgdasklgdasklsadgklgadskldasgkldasgkkjggjl";

            // Act
            string hashedByHash = hash.HashValue(password);
            string hashedSHA = "3jzY2W48r3CfiB64c/rSCBLIMGUitl0Nhygcgiry31Q=";

            // Assert
            Assert.That(hashedByHash == hashedSHA);
        }

        // TEST 14
        // Check if user is checked for correct token - correct version
        [Test]
        public void TestValidateUserTokenCorrect() {
            // Arrange
            IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
            connection.Open();
            LoggedInValidator validator = new LoggedInValidator();
            Dictionary<string, string> headers = new();
            User user = new User("kienboec", "daniel");

            // Act
            headers.Add("Authorization", $"Basic {user.Username}-mtcgToken");
            bool result = validator.ValidateCorrectToken(headers, connection, user.Username);
            connection.Close();

            // Assert
            Assert.That(result == true);
        }


        // TEST 15
        // Check if user is checked for correct token - failed version (user not logged in)
        [Test]
        public void TestValidateUserTokenNotLoggedIn() {
            // Arrange
            IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
            connection.Open();
            LoggedInValidator validator = new LoggedInValidator();
            Dictionary<string, string> headers = new();
            User user = new User("TestFU", "TestFP");

            // Act
            headers.Add("Authorization", $"Basic {user.Username}-mtcgToken");
            bool result = validator.ValidateCorrectToken(headers, connection, user.Username);
            connection.Close();

            // Assert
            Assert.That(result == false);
        }

        // TEST 16
        // Check if user is checked for correct token - user does not exist
        [Test]
        public void TestValidateUserTokenDoesNotExist() {
            // Arrange
            IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
            connection.Open();
            LoggedInValidator validator = new LoggedInValidator();
            Dictionary<string, string> headers = new();
            User user = new User("whoareyou", "doesntmatter");

            // Act
            headers.Add("Authorization", $"Basic {user.Username}-mtcgToken");
            bool result = validator.ValidateCorrectToken(headers, connection, user.Username);
            connection.Close();

            // Assert
            Assert.That(result == false);
        }


        // TEST 17
        // Check if user is checked for correct token - wrong user+token combo
        [Test]
        public void TestValidateUserTokenWrongCombo() {
            // Arrange
            IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
            connection.Open();
            LoggedInValidator validator = new LoggedInValidator();
            Dictionary<string, string> headers = new();
            User user = new User("kienboec", "daniel");
            // Act
            headers.Add("Authorization", $"Basic altenhof-mtcgToken");
            bool result = validator.ValidateCorrectToken(headers, connection, user.Username);
            connection.Close();

            // Assert
            Assert.That(result == false);
        }

        // TEST 18
        // Check if user profile is generated correctly
        [Test]
        public void TestUserProfiles() {
            // Arrange
            Card card = new Card("tid", "TestName", "Fire", "Monster", 666.0);
            UserProfile profile = new UserProfile("Tester", "This is my bio.", "XD");

            // Act
            string name = "Tester";
            string bio = "This is my bio.";
            string image = "XD";

            // Assert
            Assert.AreEqual(profile.Name, name);
            Assert.AreEqual(profile.Bio, bio);
            Assert.AreEqual(profile.Image, image);
        }
        
        // TEST 19
        // Check if database works correctly - add to user table
        [Test]
        public void TestAddUserToDatabase() {
            // Arrange
            string username = "";
            string password = "";

            Hash hash = new Hash();
            IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
            connection.Open();
            User user = new User("JustTest", "JustPassword");
            
            IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
                insert into users 
                    (username, password, token, coins, elo, games, wins, losses) 
                values
                    (@username, @password, @token, @coins, @elo, @games, @wins, @losses)";

            IDbCommand getCommand = connection.CreateCommand();
            getCommand.CommandText = @"SELECT username, password FROM users WHERE username = @username";
            
            // Act
            NpgsqlCommand c = command as NpgsqlCommand;
            c.Parameters.AddWithValue("username", user.Username);
            c.Parameters.AddWithValue("password", hash.HashValue(user.Password));
            c.Parameters.AddWithValue("token", "");
            c.Parameters.AddWithValue("coins", 20);
            c.Parameters.AddWithValue("elo", 100);
            c.Parameters.AddWithValue("games", 0);
            c.Parameters.AddWithValue("wins", 0);
            c.Parameters.AddWithValue("losses", 0);

            c.Prepare();
            command.ExecuteNonQuery();

            var pUSERNAME = getCommand.CreateParameter();
            pUSERNAME.DbType = DbType.String;
            pUSERNAME.ParameterName = "username";
            pUSERNAME.Value = user.Username;
            getCommand.Parameters.Add(pUSERNAME);

            var reader = getCommand.ExecuteReader();
            while(reader.Read()) {
                username = reader.GetString(0);
                password = reader.GetString(1);
            }

            reader.Close();
            connection.Close();

            // Assert
            Assert.AreEqual(username, user.Username);
            Assert.AreEqual(password, hash.HashValue(user.Password));
        }



        // TEST 20
        // Check if database works correctly - delete user from the table
        [Test]
        public void TestDeleteUserFromTest19() {
            // Arrange
            int rows = 0;

            IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
            connection.Open();

            IDbCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE username = @username";

            // Act
            var pUSERNAME = command.CreateParameter();
            pUSERNAME.DbType = DbType.String;
            pUSERNAME.ParameterName = "username";
            pUSERNAME.Value = "JustTest";
            command.Parameters.Add(pUSERNAME);

            rows = command.ExecuteNonQuery();

            // Assert
            Assert.That(rows == 1);
        }
        
    }
}
