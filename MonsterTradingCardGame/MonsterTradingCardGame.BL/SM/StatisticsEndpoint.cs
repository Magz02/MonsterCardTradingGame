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

namespace MonsterTradingCardGame.BL.SM {
    public class StatisticsEndpoint : IHttpEndpoint {
        public StatisticsEndpoint() {
            
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

                Stats stats = null;
                
                IDbCommand command = connection.CreateCommand();
                command.CommandText = @"select games, wins, losses, elo from users where token = @token";

                var pTOKEN = command.CreateParameter();
                pTOKEN.DbType = DbType.String;
                pTOKEN.ParameterName = "token";
                pTOKEN.Value = token;
                command.Parameters.Add(pTOKEN);

                var reader = command.ExecuteReader();
                while (reader.Read()) {
                    stats = new Stats(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
                }

                reader.Close();

                if (stats == null) {
                    throw new Exception("No stats found");
                }

                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.ResponseContent = JsonSerializer.Serialize(stats);
                rs.ContentType = "application/json";
                rs.Process();
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Could not get statistics";
                rs.ContentType = "text/plain";
                rs.Process();
            }
        }
    }
}
