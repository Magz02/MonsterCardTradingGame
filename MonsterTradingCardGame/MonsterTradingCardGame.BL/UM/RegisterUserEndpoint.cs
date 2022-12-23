using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using Npgsql;
using System.Data;
using System.Text.Json;

namespace MonsterTradingCardGame.BL.UM {
    public class RegisterUserEndpoint : IHttpEndpoint {
        public RegisterUserEndpoint() {

        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                Hash hash = new();
                var user = JsonSerializer.Deserialize<User>(rq.Content);
                if (user.Username == null || user.Password == null) {
                    throw new Exception("One of the arguments is empty");
                }
                // TODO: hash password

                // TODO: if adding new user returns fail, throw exception
                // TODO: add user via DAL - new class with an interface

                IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
                connection.Open();

                IDbCommand command = connection.CreateCommand();
                command.CommandText = @"
                insert into users 
                    (username, password, coins, token, elo) 
                values
                    (@username, @password, @coins, @token, @elo)";

                NpgsqlCommand c = command as NpgsqlCommand;
                c.Parameters.AddWithValue("username", user.Username);
                c.Parameters.AddWithValue("password", hash.HashValue(user.Password));
                c.Parameters.AddWithValue("coins", 20);
                c.Parameters.AddWithValue("token", $"Basic {user.Username}-mtcgToken");
                c.Parameters.AddWithValue("elo", 100);
                
                c.Prepare();
                command.ExecuteNonQuery();

                connection.Close();
                
                rs.ResponseCode = 201;
                rs.ResponseText = "Created";
                rs.Process();
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Failed to register new User";
                rs.ContentType = "text/plain";
                rs.Process();
            }
        }
    }
}
