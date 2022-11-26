using MonsterTradingCardGame.Model;

namespace MonsterTradingCardGame.Test {
    public class CardTest {
        [SetUp]
        public void Setup() {
        }

        // test to check whether FireSpell card is being created when requested
        [Test]
        public void FireSpellCreationTest() {
            var card = new FireSpell();
            Assert.That(card, Is.TypeOf<FireSpell>());
        }

        // test to check whether WaterSpell card is called WaterSpell
        [Test]
        public void WaterSpellCreationTest() {
            var card = new WaterSpell();
            Assert.That(card.Name, Is.EqualTo("Water Spell"));
        }

        // test to ckeck if FireSpell card has type Spell
        [Test]
        public void FireSpellTypeTest() {
            var card = new FireSpell();
            Assert.That(card.Type, Is.EqualTo(1));
        }
    }
}
