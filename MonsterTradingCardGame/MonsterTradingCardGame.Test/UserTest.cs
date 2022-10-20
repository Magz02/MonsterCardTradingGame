using MonsterTradingCardGame.Model;

namespace MonsterTradingCardGame.Test {
    public class UserTest {
        [SetUp]
        public void Setup() {
        }

        /***
          
        [Test]
        public void Test1() {
            Assert.Pass();
        }
        
        ***/

        //checks if user is created correctly
        [Test]
        public void UserCreationTest() {
            var user = new User("Rudolf", "1234");
            Assert.That(user.Name, Is.EqualTo("Rudolf"));
            Assert.That(user.Password, Is.EqualTo("1234"));
        }

        
    }
}