using MonsterTradingCardGame.BL.HTTP;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL.SM {
    public class ScoreboardEndpoint : IHttpEndpoint {
        public ScoreboardEndpoint() {
            
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

                int elo = 0;

                IDbCommand command = connection.CreateCommand();
                command.CommandText = @"select username, elo from users order by elo desc";
                
                List<string> scoreboard = new();

                int i = 1;
                var reader = command.ExecuteReader();
                while(reader.Read()) {
                    scoreboard.Add($"#{i} - {reader.GetString(0)}: {reader.GetInt32(1)} elo score");
                    i++;
                }

                reader.Close();

                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.ResponseContent = JsonSerializer.Serialize(scoreboard);
                rs.ContentType = "application/json";
                rs.Process();
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Scoreboard could not be acquired";
                rs.ContentType = "text/plain";
                rs.Process();
            }
        }
    }
}
