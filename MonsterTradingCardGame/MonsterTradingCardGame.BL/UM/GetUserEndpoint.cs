using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
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
                IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
                connection.Open();

                LoggedInValidator validator = new LoggedInValidator();
                if (!validator.Validate(rq.headers, connection)) {
                    connection.Close();
                    throw new Exception("No authorization token found or user not logged in");
                }

                Console.WriteLine("User is logged in");

                var searchedUsername = rq.Path[2];
                Console.WriteLine(searchedUsername);

                if (!validator.ValidateCorrectToken(rq.headers, connection, searchedUsername)) {
                    connection.Close();
                    throw new Exception("Wrong authorization token for this user");
                }

                IDbCommand command = connection.CreateCommand();
                command.CommandText = @"select name, bio, image from profiles where user_fun = @user_fun";

                NpgsqlCommand c = command as NpgsqlCommand;
                c.Parameters.AddWithValue("user_fun", searchedUsername);

                c.Prepare();

                UserProfile profile = null;
                var reader = command.ExecuteReader();
                while (reader.Read()) {
                    profile = new UserProfile(reader.GetString(0), reader.GetString(1), reader.GetString(2));
                }
                
                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.ResponseContent = JsonSerializer.Serialize(profile);
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
