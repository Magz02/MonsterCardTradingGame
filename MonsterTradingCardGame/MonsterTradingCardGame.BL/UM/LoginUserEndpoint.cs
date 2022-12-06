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
    public class LoginUserEndpoint : IHttpEndpoint {
        public LoginUserEndpoint() {
            
        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                Hash hash = new();
                var user = JsonSerializer.Deserialize<User>(rq.Content);

                IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
                connection.Open();

                IDbCommand command = connection.CreateCommand();
                command.CommandText = @"
                    select username, password, token from users 
                    where username = @username";

                var pUSERNAME = command.CreateParameter();
                pUSERNAME.DbType = DbType.String;
                pUSERNAME.ParameterName = "username";
                pUSERNAME.Value = user.Username;
                command.Parameters.Add(pUSERNAME);

                var reader = command.ExecuteReader();
                int entries = 0;
                while (reader.Read()) {
                    var userUsername = reader.GetString(0);
                    var userPassword = reader.GetString(1);
                    if (hash.HashValue(user.Password) != userPassword || userPassword == null) {
                        throw new Exception("Wrong password");
                        break;
                    }
                    var userToken = reader.GetString(2);
                    user.Token = userToken;
                    entries++;
                }
                
                if (entries == 0) {
                    throw new Exception("User not found");
                }

                reader.Close();
                connection.Close();
                
                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.Process();
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Failed to login User";
                rs.Process();
            }
        }
    }
}
