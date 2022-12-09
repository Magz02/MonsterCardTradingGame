using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using MonsterTradingCardGame.Model.Enums;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL.CM {
    public class AcquirePackageEndpoint : IHttpEndpoint {
        public AcquirePackageEndpoint() {

        }

        public void HandleRequest(HttpRequest rq, HttpResponse rs) {
            try {
                if (rq.headers.ContainsKey("Authorization")) {
                    // getting the package from user with token
                    var token = rq.headers["Authorization"];

                    IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
                    connection.Open();

                    // get all packages
                    IDbCommand command = connection.CreateCommand();
                    command.CommandText = @"select * from packages";

                    List<Package> packages = new();
                    var reader = command.ExecuteReader();
                    while (reader.Read()) {
                        var packageId = reader.GetInt32(0);
                        var packageCard1 = reader.GetString(1);
                        var packageCard2 = reader.GetString(2);
                        var packageCard3 = reader.GetString(3);
                        var packageCard4 = reader.GetString(4);
                        var packageCard5 = reader.GetString(5);
                        var packageToAdd = new Package(packageId, packageCard1, packageCard2, packageCard3, packageCard4, packageCard5);
                        packages.Add(packageToAdd);
                    }

                    if (packages == null) {
                        throw new Exception("No packages found");
                    }

                    reader.Close();
                    // TODO: Look after the username and coins using authorization token - if insufficient amount of coins - exception
                    IDbCommand userCommand = connection.CreateCommand();
                    userCommand.CommandText = @"
                        select username, coins from users
                        where token = @token";

                    var pTOKEN = userCommand.CreateParameter();
                    pTOKEN.DbType = DbType.String;
                    pTOKEN.ParameterName = "token";
                    pTOKEN.Value = token;
                    userCommand.Parameters.Add(pTOKEN);

                    reader = userCommand.ExecuteReader();

                    string username = "";
                    int coins = 0;

                    while (reader.Read()) {
                        username = reader.GetString(0);
                        coins = reader.GetInt32(1);
                    }

                    if (coins <= 0) {
                        throw new Exception("Insufficient credit");
                    }

                    if (username == "") {
                        throw new Exception("No user found");
                    }

                    reader.Close();

                    Console.WriteLine($"Package to be added: {packages[0].Card1id}, {packages[0].Card2id}, {packages[0].Card3id}, {packages[0].Card4id}, {packages[0].Card5id}");
                    // delete package from
                    // packages and update user table for coins (authorization method used earlier)

                    string[] cardids = new string[5];
                    cardids[0] = packages[0].Card1id;
                    cardids[1] = packages[0].Card2id;
                    cardids[2] = packages[0].Card3id;
                    cardids[3] = packages[0].Card4id;
                    cardids[4] = packages[0].Card5id;

                    IDbCommand addToDeckCommand = connection.CreateCommand();
                    
                    foreach (var card in cardids) {
                        // TODO: PSQL constraints
                        addToDeckCommand.CommandText = @"
                            insert into decks (card_id, owner_name)
                            values (@cardid, @ownername)";

                        NpgsqlCommand c = addToDeckCommand as NpgsqlCommand;
                        c.Parameters.AddWithValue("cardid", card);
                        c.Parameters.AddWithValue("ownername", username);

                        c.Prepare();
                        addToDeckCommand.ExecuteNonQuery();
                    }

                    // Update user coins, take 5 away
                    coins = coins - 5;

                    IDbCommand updateCoins = connection.CreateCommand();
                    updateCoins.CommandText = @"
                        update users
                        set coins = @coins
                        where username = @username";

                    NpgsqlCommand cU = updateCoins as NpgsqlCommand;
                    cU.Parameters.AddWithValue("coins", coins);
                    cU.Parameters.AddWithValue("username", username);

                    cU.Prepare();
                    cU.ExecuteNonQuery();

                    // delete package from available packages
                    IDbCommand deletePackage = connection.CreateCommand();
                    deletePackage.CommandText = @"
                        delete from packages
                        where id = @id";
                    
                    NpgsqlCommand d = deletePackage as NpgsqlCommand;
                    d.Parameters.AddWithValue("id", packages[0].Id);

                    d.Prepare();
                    d.ExecuteNonQuery();


                    connection.Close();
                    rs.ResponseCode = 200;
                    rs.ResponseText = "OK";
                    rs.Process();
                } else {
                    rs.ResponseCode = 401;
                    rs.ResponseText = "Unauthorized";
                    rs.ResponseContent = "Failed to authorize the user. No token given.";
                    rs.Process();
                }
            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Failed to acquire Package";
                rs.Process();
            }
        }
    }
}
