using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL.CM {
    public class CreatePackageEndpoint : IHttpEndpoint {
        public CreatePackageEndpoint() {
            
        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                var cards = JsonSerializer.Deserialize<Card>(rq.Content);
                rs.ResponseCode = 201;
                rs.ResponseText = "Created";
                // here are the card created as a package
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Failed to create Package";
                rs.Process();
            }
        }
    }
}
