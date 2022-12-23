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
                if (!rq.headers.ContainsKey("Authorization")) {
                    throw new Exception("No authorization token found");
                }

                /*if (rq.headers["Authorization"] != rq.QueryParams["token"]) {
                    throw new Exception("Authorization token does not match the token in the query params");
                }*/
                
                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.ResponseContent = "User updated";
                rs.ContentType = "text/plain";
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Failed to update User";
                rs.ContentType = "text/plain";
                rs.Process();
            }
        }
    }
}
