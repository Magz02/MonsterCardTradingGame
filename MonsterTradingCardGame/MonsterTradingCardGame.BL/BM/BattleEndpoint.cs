using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL.BM {
    public class BattleEndpoint : IHttpEndpoint {

        public BattleEndpoint() {

        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                if (rq.headers["Authorization"] == null) {
                    throw new Exception("No authorization token found");
                }

                var token = rq.headers["Authorization"];

                IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
                connection.Open();

                List<User> allUsersExceptUser = new();

                IDbCommand allUsersCommand = connection.CreateCommand();
                allUsersCommand.CommandText = @"select * from users where token != @token";

                var pTOKEN = allUsersCommand.CreateParameter();
                pTOKEN.DbType = DbType.String;
                pTOKEN.ParameterName = "token";
                pTOKEN.Value = token;
                allUsersCommand.Parameters.Add(pTOKEN);

                var reader = allUsersCommand.ExecuteReader();
                while (reader.Read()) {
                    var userId = reader.GetInt32(0);
                    var userUsername = reader.GetString(1);
                    var userPassword = reader.GetString(2);
                    var userCoins = reader.GetInt32(3);
                    var userToken = reader.GetString(4);
                    var userElo = reader.GetInt32(5);
                    var userToAdd = new User(userUsername, userPassword);
                    userToAdd.Elo = userElo;
                    userToAdd.Coins = userCoins;
                    userToAdd.Token = userToken;
                    userToAdd.Id = userId;
                    allUsersExceptUser.Add(userToAdd);
                }

                foreach (var that in allUsersExceptUser) {
                    Console.WriteLine($"User: {that.Id} - {that.Username}. Coins: {that.Coins}. Elo: {that.Elo}");
                }

                reader.Close();
                connection.Close();

                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.ResponseContent = "Battle finished - placeholder of a message";
                rs.ContentType = "text/plain";
                rs.Process();

            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Cards could not be acquired";
                rs.ContentType = "text/plain";
                rs.Process();
            }
        }

    }
}
