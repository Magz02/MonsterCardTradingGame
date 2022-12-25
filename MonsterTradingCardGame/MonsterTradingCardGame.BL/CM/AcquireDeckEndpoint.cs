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
    public class AcquireDeckEndpoint : IHttpEndpoint {
        public AcquireDeckEndpoint() {

        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                if (rq.headers["Authorization"] == null) {
                    throw new Exception("No Authorization");
                }

                List<Card> allCards = new();
                List<Card> chosenDeck = new();
                var token = rq.headers["Authorization"];

                IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
                connection.Open();

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

                IDbCommand commandAllCards = connection.CreateCommand();
                commandAllCards.CommandText = @"select id, name, type, element, damage, chosen from cards where owner_name = @owner_name order by damage desc";

                var pOWNER = commandAllCards.CreateParameter();
                pOWNER.DbType = DbType.String;
                pOWNER.ParameterName = "owner_name";
                pOWNER.Value = username;
                commandAllCards.Parameters.Add(pOWNER);

                reader = commandAllCards.ExecuteReader();
                while (reader.Read()) {
                    var id = reader.GetString(0);
                    var name = reader.GetString(1);
                    var type = reader.GetString(2);
                    var element = reader.GetString(3);
                    var damage = reader.GetDouble(4);
                    var isChosen = reader.GetBoolean(5);
                    Card toAdd = new Card(id, name, element, type, damage);
                    if (isChosen) {
                        chosenDeck.Add(toAdd);
                    } else {
                        allCards.Add(toAdd);
                    }
                }

                var deckJson = "";

                while (chosenDeck.Count < 4) {
                    chosenDeck.Add(allCards[0]);
                    allCards.RemoveAt(0);
                }

                if (chosenDeck.Count > 4) {
                    throw new Exception("Deck is too big");
                }

                if (chosenDeck.Count == 4) {
                    deckJson = JsonSerializer.Serialize(chosenDeck);
                }

                rs.ResponseCode = 200;
                rs.ResponseText = "OK";

                // if queryparams has "format" as a key and it's value is "plain" then return plain text
                if (rq.QueryParams.ContainsKey("format") && rq.QueryParams["format"] == "plain") {
                    rs.ContentType = "text/plain";
                    rs.ResponseContentList = new List<string>();
                    foreach (var card in chosenDeck) {
                        rs.ResponseContentList.Add($"{card.Id}: {card.Name} (e: {card.Element}, t: {card.Type}, dmg: {card.Damage})");
                    }
                    rs.Process();
                    return;
                }

                /*if (rq.QueryParams["format"] == "plain") {
                    //TODO : RESPONSE CONTENT AS STRING
                    rs.ContentType = "text/plain";
                    return;
                }*/
                rs.ResponseContent = deckJson;
                rs.ContentType = "application/json";
                rs.Process();

            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad request";
                rs.ResponseContent = "Could not acquire deck";
                rs.ContentType = "text/plain";
                rs.Process();
            }
        }
    }
}