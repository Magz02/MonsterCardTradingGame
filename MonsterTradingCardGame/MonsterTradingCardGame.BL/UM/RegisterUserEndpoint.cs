using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using System.Text.Json;

namespace MonsterTradingCardGame.BL.UM {
    public class RegisterUserEndpoint : IHttpEndpoint {
        public RegisterUserEndpoint() {

        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                var user = JsonSerializer.Deserialize<User>(rq.Content);
                rs.ResponseCode = 201;
                rs.ResponseText = "Created";
                rs.Process();
                // here is the user created- DB
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Failed to register new User";
                rs.Process();
            }
        }
    }
}
