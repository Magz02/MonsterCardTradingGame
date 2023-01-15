using MonsterTradingCardGame.BL.HTTP;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL.UM {
    public class LogoutUserEndpoint : IHttpEndpoint {
        public LogoutUserEndpoint() {

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

                IDbCommand command = connection.CreateCommand();
                command.CommandText = @"
                    update users set token = ''
                    where token = @token";

                var pTOKEN = command.CreateParameter();
                pTOKEN.DbType = DbType.String;
                pTOKEN.ParameterName = "token";
                pTOKEN.Value = token;
                command.Parameters.Add(pTOKEN);

                command.ExecuteNonQuery();

                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.ResponseContent = $"You have been logged out. User token {token} has been deleted";
                rs.ContentType = "text/plain";
                rs.Process();
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Failed to login User";
                rs.ContentType = "text/plain";
                rs.Process();
            }
        }
    }
}
