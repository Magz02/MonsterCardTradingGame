using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL.UM {
    public class LoginUserEndpoint : IHttpEndpoint {
        public LoginUserEndpoint() {
            
        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                var user = JsonSerializer.Deserialize<User>(rq.Content);
                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                // here comes login
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Failed to login User";
                rs.Process();
            }
        }
    }
}
