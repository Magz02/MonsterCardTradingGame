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

namespace MonsterTradingCardGame.BL.CM {
    public class EditDeckEndpoint : IHttpEndpoint {
        public EditDeckEndpoint() {
        
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

                List<string> desiredDeck = JsonSerializer.Deserialize<List<string>>(rq.Content);
                var token = rq.headers["Authorization"];

                if (desiredDeck == null || desiredDeck.Count != 4) {
                    throw new Exception("Invalid deck");
                }

                IDbCommand commandUsername = connection.CreateCommand();
                commandUsername.CommandText = @"select username from users where token = @token";
                var pTOKEN = commandUsername.CreateParameter();
                pTOKEN.DbType = DbType.String;
                pTOKEN.ParameterName = "token";
                pTOKEN.Value = token;
                commandUsername.Parameters.Add(pTOKEN);

                var reader = commandUsername.ExecuteReader();
                var username = "";

                while (reader.Read()) {
                    username = reader.GetString(0);
                }

                if (username == "") {
                    throw new Exception("No username found");
                }

                reader.Close();

                IDbCommand deleteDeckCommand = connection.CreateCommand();
                deleteDeckCommand.CommandText = @"update cards set chosen = false where owner_name = @owner_name";

                var pOWNER = deleteDeckCommand.CreateParameter();
                pOWNER.DbType = DbType.String;
                pOWNER.ParameterName = "owner_name";
                pOWNER.Value = username;
                deleteDeckCommand.Parameters.Add(pOWNER);

                deleteDeckCommand.ExecuteNonQuery();

                // verify if the cards exist loop
                foreach (var cardId in desiredDeck) {
                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = @"select id from cards where id = @id and owner_name = @owner_name";

                    NpgsqlCommand c = command as NpgsqlCommand;
                    c.Parameters.AddWithValue("id", cardId);
                    c.Parameters.AddWithValue("owner_name", username);

                    var reader2 = command.ExecuteReader();
                    while(reader2.Read()) {
                        Console.WriteLine("Found");
                    }

                    reader2.Close();
                }

                // update all cards
                foreach (var cardId in desiredDeck) {
                    IDbCommand updateDeckCommand = connection.CreateCommand();
                    updateDeckCommand.CommandText = @"update cards set chosen = true where owner_name = @owner_name and id = @id";

                    NpgsqlCommand c = updateDeckCommand as NpgsqlCommand;
                    c.Parameters.AddWithValue("owner_name", username);
                    c.Parameters.AddWithValue("id", cardId);

                    c.Prepare();
                    updateDeckCommand.ExecuteNonQuery();
                }

                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.ResponseContent = "Deck updated";
                rs.ContentType = "text/plain";
                rs.Process();

            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Could not edit deck";
                rs.ContentType = "text/plain";
                rs.Process();
            }
        }
    }
}
