using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL.UM {
    public class EditUserEndpoint : IHttpEndpoint {
        public EditUserEndpoint() {

        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
                connection.Open();

                LoggedInValidator validator = new LoggedInValidator();
                if (!validator.Validate(rq.headers, connection)) {
                    connection.Close();
                    throw new Exception("No authorization token found or user not logged in");
                }

                var token = rq.headers["Authorization"];

                // TODO: Add new columns to db

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
