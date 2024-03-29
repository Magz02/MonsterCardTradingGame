﻿using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL.CM {
    public class CreatePackageEndpoint : IHttpEndpoint {
        public CreatePackageEndpoint() {
            
        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
                connection.Open();

                LoggedInValidator validator = new LoggedInValidator();
                if (!validator.ValidateAdmin(rq.headers, connection)) {
                    connection.Close();
                    throw new Exception("No authorization token found or admin not logged in");
                }
                
                var cardsJson = JsonNode.Parse(rq.Content);
                Console.WriteLine(cardsJson.ToString());
                Console.WriteLine(cardsJson[0].ToString());
                Console.WriteLine(cardsJson[0]["Damage"]);
                if (cardsJson[4] == null) {
                    throw new Exception("No valid cards package found");
                }
                string[] packageIds = new string[5];

                for (int i = 0; i < 5; i++) {
                    Card currentCard = new Card(cardsJson[i]["Id"].ToString(), cardsJson[i]["Name"].ToString(), cardsJson[i]["Element"].ToString(), cardsJson[i]["Type"].ToString(), ((double)cardsJson[i]["Damage"]));
                    Console.WriteLine($"Id of the {i} card: {currentCard.Id}");
                    Console.WriteLine($"Name of the {i} card: {currentCard.Name}");
                    Console.WriteLine($"Element of the {i} card: {currentCard.Element}");
                    Console.WriteLine($"Type of the {i} card: {currentCard.Type}");
                    Console.WriteLine($"Damage of the {i} card: {currentCard.Damage}");
                    currentCard.Owner = "package";
                    currentCard.Chosen = false;
                    AddCardToCardDB(currentCard, connection);
                    packageIds[i] = currentCard.Id;
                }
                
                AddCardstoPackageDB(packageIds, connection);
                connection.Close();

                rs.ResponseCode = 201;
                rs.ResponseText = "Created";
                rs.Process();
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Failed to create Package";
                rs.ContentType = "text/plain";
                rs.Process();
            }
        }


        private void AddCardToCardDB(Card currentCard, IDbConnection connection) {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
            insert into cards
                (id, name, type, element, damage, owner_name, chosen)
            values
                (@id, @name, @type, @element, @damage, @owner_name, @chosen)";

            string elementDB = currentCard.Element.ToString();
            string typeDB = currentCard.Type.ToString();

            NpgsqlCommand c = command as NpgsqlCommand;
            c.Parameters.AddWithValue("id", currentCard.Id);
            c.Parameters.AddWithValue("name", currentCard.Name);
            c.Parameters.AddWithValue("type", typeDB);
            c.Parameters.AddWithValue("element", elementDB);
            c.Parameters.AddWithValue("damage", currentCard.Damage);
            c.Parameters.AddWithValue("owner_name", currentCard.Owner);
            c.Parameters.AddWithValue("chosen", currentCard.Chosen);

            c.Prepare();
            command.ExecuteNonQuery();

            Console.WriteLine("Card added to DB");
        }
        
        private void AddCardstoPackageDB(string[] package, IDbConnection connection) {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = @"
            insert into packages
                (card1id, card2id, card3id, card4id, card5id)
            values
                (@card1id, @card2id, @card3id, @card4id, @card5id)";

            NpgsqlCommand c = command as NpgsqlCommand;
            c.Parameters.AddWithValue("card1id", package[0]);
            c.Parameters.AddWithValue("card2id", package[1]);
            c.Parameters.AddWithValue("card3id", package[2]);
            c.Parameters.AddWithValue("card4id", package[3]);
            c.Parameters.AddWithValue("card5id", package[4]);

            c.Prepare();
            command.ExecuteNonQuery();

            Console.WriteLine("Package added to DB");
        }
    }
}
