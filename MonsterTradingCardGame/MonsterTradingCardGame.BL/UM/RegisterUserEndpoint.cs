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
                Hash hash = new Hash();
                var user = JsonSerializer.Deserialize<User>(rq.Content);
                if (user.Username == null || user.Password == null) {
                    throw new Exception("One of the arguments is empty");
                }

                IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
                connection.Open();

                IDbCommand command = connection.CreateCommand();
                command.CommandText = @"
                insert into users 
                    (username, password, token, coins, elo, games, wins, losses) 
                values
                    (@username, @password, @token, @coins, @elo, @games, @wins, @losses)";

                NpgsqlCommand c = command as NpgsqlCommand;
                c.Parameters.AddWithValue("username", user.Username);
                c.Parameters.AddWithValue("password", hash.HashValue(user.Password));
                c.Parameters.AddWithValue("token", "");
                c.Parameters.AddWithValue("coins", 20);
                c.Parameters.AddWithValue("elo", 100);
                c.Parameters.AddWithValue("games", 0);
                c.Parameters.AddWithValue("wins", 0);
                c.Parameters.AddWithValue("losses", 0);

                c.Prepare();
                command.ExecuteNonQuery();

                IDbCommand profileCommand = connection.CreateCommand();
                profileCommand.CommandText = @"insert into profiles (user_fun, name, bio, image) values (@user_fun, @name, @bio, @image)";

                NpgsqlCommand profileC = profileCommand as NpgsqlCommand;
                profileC.Parameters.AddWithValue("user_fun", user.Username);
                profileC.Parameters.AddWithValue("name", "");
                profileC.Parameters.AddWithValue("bio", "");
                profileC.Parameters.AddWithValue("image", "");
                
                profileC.Prepare();
                profileCommand.ExecuteNonQuery();

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
