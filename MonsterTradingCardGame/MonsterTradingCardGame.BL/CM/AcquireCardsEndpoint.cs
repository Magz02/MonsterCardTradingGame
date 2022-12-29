using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using MonsterTradingCardGame.Model.Enums;
using Npgsql;
using System.Data;
using System.Text.Json;

namespace MonsterTradingCardGame.BL.CM {
    public class AcquireCardsEndpoint : IHttpEndpoint {
        public AcquireCardsEndpoint() {

        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                if (!rq.headers.ContainsKey("Authorization")) {
                    throw new Exception("No authorization token found");
                }

                var token = rq.headers["Authorization"];
                List<Card> cards = new();

                IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
                connection.Open();

                IDbCommand getOwnerCommand = connection.CreateCommand();
                getOwnerCommand.CommandText = @"select username from users where token = @token";

                var pTOKEN = getOwnerCommand.CreateParameter();
                pTOKEN.DbType = DbType.String;
                pTOKEN.ParameterName = "token";
                pTOKEN.Value = token;
                getOwnerCommand.Parameters.Add(pTOKEN);

                var username = "";

                var reader = getOwnerCommand.ExecuteReader();
                while (reader.Read()) {
                    username = reader.GetString(0);
                }

                reader.Close();

                if (username == "") {
                    throw new Exception("No user found");
                }

                IDbCommand command = connection.CreateCommand();
                command.CommandText = @"
                    select id, name, type, element, damage from cards
                    where owner_name = @owner_name";
                
                var pOWNER = command.CreateParameter();
                pOWNER.DbType = DbType.String;
                pOWNER.ParameterName = "owner_name";
                pOWNER.Value = username;
                command.Parameters.Add(pOWNER);

                int i = 1;

                reader = command.ExecuteReader();
                while (reader.Read()) {
                    var cardId = reader.GetString(0);
                    var cardName = reader.GetString(1);
                    var cardType = reader.GetString(2);
                    var cardElement = reader.GetString(3);
                    var cardDamage = reader.GetDouble(4);
                    Console.WriteLine($"Card no.{i}: {cardId}/{cardName} (Type: {cardType}, Element: {cardElement}, Damage: {cardDamage})");
                    var cardToAdd = new Card(cardId, cardName, cardElement, cardType, cardDamage);
                    cards.Add(cardToAdd);
                    i++;
                }

                if (cards == null) {
                    throw new Exception("No cards found");
                }
                
                var cardsJson = JsonSerializer.Serialize(cards);

                reader.Close();
                connection.Close();
                
                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.ResponseContent = cardsJson;
                rs.ContentType = "application/json";
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