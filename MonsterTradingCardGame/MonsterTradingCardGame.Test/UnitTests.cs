using MonsterTradingCardGame.Model;
using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model.Enums;
using System.Data;
using Npgsql;
using MonsterTradingCardGame.BL;

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
        public void LoginValidatorAuthorizedCheck() {
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
        public void LoginValidatorNonExistingUser() {
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
        public void LoginValidatorAuthorizedButNotLoggedInCheck() {
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
        public void LoginValidatorNotAuthorized() {
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
        public void LoginValidatorIsAdmin() {
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
        public void LoginValidatorIsNotAdmin() {
            // Arrange
            IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
            connection.Open();
            LoggedInValidator validator = new LoggedInValidator();
            Dictionary<string, string> headers = new();
            User user = new User("kienboec", "daniel");

            // Act
            headers.Add("Authorization", $"Basic {user.Username}-mtcgToken");
            bool result = validator.Validate(headers, connection);
            connection.Close();

            // Assert
            Assert.That(result == false);
        }
    }
}
