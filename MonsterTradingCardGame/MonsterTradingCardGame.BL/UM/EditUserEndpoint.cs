using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL.UM {
    public class EditUserEndpoint : IHttpEndpoint {
        public EditUserEndpoint() {

        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                // send query to DB to update user - with rq.content
                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.ResponseContent = "User updated";
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Failed to update User";
                rs.Process();
            }
        }
    }
}
