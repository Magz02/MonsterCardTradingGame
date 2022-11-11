using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using MonsterTradingCardGame.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL.CM {
    public class AcquirePackageEndpoint : IHttpEndpoint {
        public AcquirePackageEndpoint() {

        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                //getting the package
                //here is a temp package
                Package package = new Package();
                package.Cards.Add(new Card("TestCard", Model.Enums.Type.Monster, Element.Neutral, 10));
                package.Cards.Add(new Card("TestCardFire", Model.Enums.Type.Spell, Element.Fire, 20));
                package.Cards.Add(new Card("TestCardWater", Model.Enums.Type.Monster, Element.Water, 55));
                var packageJson = JsonSerializer.Serialize(package);
                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.ResponseContent = packageJson;
                rs.Process();
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Failed to acquire Package";
                rs.Process();
            }
        }
    }
}
