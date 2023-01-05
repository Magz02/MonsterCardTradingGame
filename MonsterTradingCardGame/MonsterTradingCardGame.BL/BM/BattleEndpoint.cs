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
                    var userUsername = reader.GetString(1);
                    var userPassword = reader.GetString(2);
                    var userToAdd = new User(userUsername, userPassword);
                    userToAdd.Id = reader.GetInt32(0);
                    userToAdd.Coins = reader.GetInt32(3);
                    userToAdd.Token = reader.GetString(4);
                    userToAdd.Elo = reader.GetInt32(5);
                    userToAdd.Games = reader.GetInt32(6);
                    userToAdd.Wins = reader.GetInt32(7);
                    userToAdd.Losses = reader.GetInt32(8);
                    allUsersExceptUser.Add(userToAdd);
                }

                Random rnd = new Random();
                int num = rnd.Next() % allUsersExceptUser.Count;

                User opponent = allUsersExceptUser[num];
                Console.WriteLine($"Opponent chosen: {opponent.Id} - {opponent.Username}. Coins: {opponent.Coins}. Elo: {opponent.Elo}");

                Card[] userDeck = new Card[4];
                Card[] opponentDeck = new Card[4];

                User user = getUser(token, connection);
                userDeck = getDeck(user, connection);
                opponentDeck = getDeck(opponent, connection);

                int i = 1;

                while (userDeck.Length > 0 && opponentDeck.Length > 0) {
                    Console.WriteLine($"Round {i}");
                    fight(userDeck, opponentDeck, user, opponent, connection);
                    i++;
                }

                // final steps
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

        private User getUser(string token, IDbConnection connection) {
            IDbCommand commandUsername = connection.CreateCommand();
            commandUsername.CommandText = @"select * from users where token = @token";

            var pTOKEN = commandUsername.CreateParameter();
            pTOKEN.DbType = DbType.String;
            pTOKEN.ParameterName = "token";
            pTOKEN.Value = token;
            commandUsername.Parameters.Add(pTOKEN);

            var reader = commandUsername.ExecuteReader();
            User user = null;

            while (reader.Read()) {
                var userUsername = reader.GetString(1);
                var userPassword = reader.GetString(2);
                user = new User(userUsername, userPassword);
                user.Id = reader.GetInt32(0);
                user.Coins = reader.GetInt32(3);
                user.Token = reader.GetString(4);
                user.Elo = reader.GetInt32(5);
                user.Games = reader.GetInt32(6);
                user.Wins = reader.GetInt32(7);
                user.Losses = reader.GetInt32(8);
            }

            if (user == null) {
                throw new Exception("No user found");
            }

            reader.Close();
            return user;
        }

        private User fight(Card[] userDeck, Card[] opponentDeck, User user, User opponent, IDbConnection connection) {
            throw new NotImplementedException();
            // choose random card from userDeck
            Random rnd = new Random();
            int num = rnd.Next() % userDeck.Length;
            Card userCard = userDeck[num];
            Console.WriteLine($"User card: {userCard.Name}");

            // choose random card from opponentDeck
            num = rnd.Next() % opponentDeck.Length;
            Card opponentCard = opponentDeck[num];
            Console.WriteLine($"Opponent card: {opponentCard.Name}");

            User winner = null;
            User loser = null;

            // specialties
            winner = considerSpecial(userCard, opponentCard, user, opponent);

            if (winner != null) {
                Console.WriteLine($"Winner is {winner.Username}");
                eloUpdate(winner, loser, connection);
                return winner;
            }

            // compare two cards - 
            if (userCard.Type == Model.Enums.Type.Monster && opponentCard.Type == Model.Enums.Type.Monster) {
                winner = monsterVsMonster(userCard, opponentCard, user, opponent);
            } else if (userCard.Type == Model.Enums.Type.Spell && opponentCard.Type == Model.Enums.Type.Spell) {
                winner = spellVsSpell(userCard, opponentCard, user, opponent);
            } else if (userCard.Type == Model.Enums.Type.Spell && opponentCard.Type == Model.Enums.Type.Monster) {
                winner = spellVsMonster(userCard, opponentCard, user, opponent);
            } else if (userCard.Type == Model.Enums.Type.Monster && opponentCard.Type == Model.Enums.Type.Spell) {
                winner = monsterVsSpell(userCard, opponentCard, user, opponent);
            }

            // set winner
            gamesUpdate(user, opponent, connection);
            
            if (winner == null) {
                Console.WriteLine($"No winner! Draw!");
                return null;
            }

            eloUpdate(winner, loser, connection);
            return winner;
        }

        private void gamesUpdate(User user, User opponent, IDbConnection connection) {
            user.Games++;
            opponent.Games++;

            gamesCommandUpdate(user, connection);
            gamesCommandUpdate(opponent, connection);
        }

        private void gamesCommandUpdate(User user, IDbConnection connection) {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = @"update users set games = @games where username = @username";

            var pGAMES = command.CreateParameter();
            pGAMES.DbType = DbType.Int32;
            pGAMES.ParameterName = "games";
            pGAMES.Value = user.Games;
            command.Parameters.Add(pGAMES);

            var pUSERNAME = command.CreateParameter();
            pUSERNAME.DbType = DbType.String;
            pUSERNAME.ParameterName = "username";
            pUSERNAME.Value = user.Username;
            command.Parameters.Add(pUSERNAME);

            command.ExecuteNonQuery();
        }

        private void eloUpdate(User winner, User loser, IDbConnection connection) {
            int winsWinner = winner.Wins;
            int lossesWinner = winner.Losses;
            int gamesWinner = winner.Games;

            int winsLoser = loser.Wins;
            int lossesLoser = loser.Losses;
            int gamesLoser = loser.Games;

            int eloWinner = winner.Elo;
            int eloLoser = loser.Elo;

            int eloOpponentWinner = getOpponentElo(winner, connection);
            int eloOpponentLoser = getOpponentElo(loser, connection);

            int eloWinnerNew = calculateElo(eloOpponentWinner, winsWinner, lossesWinner, gamesWinner);
            int eloLoserNew = calculateElo(eloOpponentLoser, winsLoser, lossesLoser, gamesLoser);

            eloCommandUpdate(winner, eloWinnerNew, connection);
            eloCommandUpdate(loser, eloLoserNew, connection);
        }

        private void eloCommandUpdate(User user, int eloNew, IDbConnection connection) {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = @"update users set elo = @elo where username = @username";

            var pELO = command.CreateParameter();
            pELO.DbType = DbType.Int32;
            pELO.ParameterName = "elo";
            pELO.Value = eloNew;
            command.Parameters.Add(pELO);

            var pUSERNAME = command.CreateParameter();
            pUSERNAME.DbType = DbType.String;
            pUSERNAME.ParameterName = "username";
            pUSERNAME.Value = user.Username;
            command.Parameters.Add(pUSERNAME);

            command.ExecuteNonQuery();
        }

        private int calculateElo(int opponentRating, int wins, int losses, int games) {
            return (opponentRating + (400 * (wins - losses))) / games;
        }

        private int getOpponentElo(User user, IDbConnection connection) {
            int elo = 0;
            
            IDbCommand commandOpponentElo = connection.CreateCommand();
            commandOpponentElo.CommandText = @"select elo from users where username != @username";

            var pUSERNAME = commandOpponentElo.CreateParameter();
            pUSERNAME.DbType = DbType.String;
            pUSERNAME.ParameterName = "username";
            pUSERNAME.Value = user.Username;
            commandOpponentElo.Parameters.Add(pUSERNAME);

            var reader = commandOpponentElo.ExecuteReader();
            while (reader.Read()) {
                elo += reader.GetInt32(0);
            }

            reader.Close();
            return elo;
        }

        private User considerSpecial(Card userCard, Card opponentCard, User user, User opponent) {
            if ((userCard.Name == "Goblin" && opponentCard.Name == "Dragon") || (userCard.Name == "Dragon" && opponentCard.Name == "Goblin")) {
                return userCard.Name == "Goblin" ? opponent : user;
            } else if ((userCard.Name == "Wizard" && opponentCard.Name == "Ork") || (userCard.Name == "Ork" && opponentCard.Name == "Wizard")) {
                return userCard.Name == "Wizard" ? user : opponent;
            } else if ((userCard.Name == "Knight" && opponentCard.Name == "WaterSpell") || (userCard.Name == "WaterSpell" && opponentCard.Name == "Knight")) {
                return userCard.Name == "Knight" ? opponent : user;
            } else if ((userCard.Name == "Kraken" && opponentCard.Type == Model.Enums.Type.Spell) || (userCard.Type == Model.Enums.Type.Spell && opponentCard.Name == "Kraken")) {
                return userCard.Name == "Kraken" ? user : opponent;
            } else if ((userCard.Name == "FireElves" && opponentCard.Name == "Dragon") || (userCard.Name == "Dragon" && opponentCard.Name == "FireElves")) {
                return userCard.Name == "FireElves" ? user : opponent;
            }

            return null;
        }

        private User monsterVsSpell(Card userCard, Card opponentCard, User user, User opponent) {
            throw new NotImplementedException();
        }

        private User spellVsMonster(Card userCard, Card opponentCard, User user, User opponent) {
            throw new NotImplementedException();
        }

        private User spellVsSpell(Card userCard, Card opponentCard, User user, User opponent) {
            throw new NotImplementedException();
        }

        private User monsterVsMonster(Card userCard, Card opponentCard, User user, User opponent) {
            throw new NotImplementedException();
        }

        private Card[] getDeck(User user, IDbConnection connection) {
            List<Card> allCards = new();
            Card[] chosenDeck = new Card[4];
            
            IDbCommand commandUserDeck = connection.CreateCommand();
            commandUserDeck.CommandText = @"select id, name, type, element, damage, chosen from cards where owner_name = @owner_name order by damage desc";

            var pOWNER = commandUserDeck.CreateParameter();
            pOWNER.DbType = DbType.String;
            pOWNER.ParameterName = "owner_name";
            pOWNER.Value = user.Username;
            commandUserDeck.Parameters.Add(pOWNER);

            var reader = commandUserDeck.ExecuteReader();
            while (reader.Read()) {
                var id = reader.GetString(0);
                var name = reader.GetString(1);
                var type = reader.GetString(2);
                var element = reader.GetString(3);
                var damage = reader.GetDouble(4);
                var isChosen = reader.GetBoolean(5);
                Card toAdd = new Card(id, name, element, type, damage);
                if (isChosen) {
                    chosenDeck.Append(toAdd);
                } else {
                    allCards.Add(toAdd);
                }
            }

            while (chosenDeck.Length < 4) {
                chosenDeck.Append(allCards[0]);
                allCards.RemoveAt(0);
            }

            if (chosenDeck.Length > 4) {
                throw new Exception("Deck is too big");
            }

            reader.Close();
            
            return chosenDeck;
        }
    }
}
