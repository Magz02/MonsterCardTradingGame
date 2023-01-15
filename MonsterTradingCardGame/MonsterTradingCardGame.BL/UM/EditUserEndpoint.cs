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

                Console.WriteLine("User is logged in");

                var searchedUsername = rq.Path[2];
                Console.WriteLine(searchedUsername);

                if (!validator.ValidateCorrectToken(rq.headers, connection, searchedUsername)) {
                    connection.Close();
                    throw new Exception("Wrong authorization token for this user");
                }

                UserProfile profile = JsonSerializer.Deserialize<UserProfile>(rq.Content);

                IDbCommand command = connection.CreateCommand();
                command.CommandText = @"update profiles set name = @name, bio = @bio, image = @image where user_fun = @user_fun";

                var pNAME = command.CreateParameter();
                pNAME.DbType = DbType.String;
                pNAME.ParameterName = "name";
                pNAME.Value = profile.Name;
                command.Parameters.Add(pNAME);

                var pBIO = command.CreateParameter();
                pBIO.DbType = DbType.String;
                pBIO.ParameterName = "bio";
                pBIO.Value = profile.Bio;
                command.Parameters.Add(pBIO);

                var pIMAGE = command.CreateParameter();
                pIMAGE.DbType = DbType.String;
                pIMAGE.ParameterName = "image";
                pIMAGE.Value = profile.Image;
                command.Parameters.Add(pIMAGE);

                var pUSER_FUN = command.CreateParameter();
                pUSER_FUN.DbType = DbType.String;
                pUSER_FUN.ParameterName = "user_fun";
                pUSER_FUN.Value = searchedUsername;
                command.Parameters.Add(pUSER_FUN);

                command.Prepare();
                command.ExecuteNonQuery();

                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.ResponseContent = "User updated";
                rs.ContentType = "text/plain";
                rs.Process();
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
