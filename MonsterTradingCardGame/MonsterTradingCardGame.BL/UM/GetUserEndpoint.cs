using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL.UM {
    public class GetUserEndpoint : IHttpEndpoint {
        public GetUserEndpoint() {

        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                // getting the user
                // here is temp user
                User user = new User("TestUser", "TestPassword");
                var userJson = JsonSerializer.Serialize(user);
                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.ResponseContent = userJson;
                rs.ContentType = "application/json";
                rs.Process();
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Failed to get User";
                rs.ContentType = "text/plain";
                rs.Process();
            }
        }
    }
}
