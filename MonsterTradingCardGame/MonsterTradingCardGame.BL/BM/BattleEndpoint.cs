using MonsterTradingCardGame.BL.HTTP;
using MonsterTradingCardGame.Model;
using MonsterTradingCardGame.Model.Logger;
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
                IDbConnection connection = new NpgsqlConnection("Host=localhost;Username=swe1user;Password=swe1pw;Database=swe1db");
                connection.Open();
                
                LoggedInValidator validator = new LoggedInValidator();
                if (!validator.Validate(rq.headers, connection)) {
                    connection.Close();
                    throw new Exception("No authorization token found or user not logged in");
                }
                
                Logger logger = new();
                var token = rq.headers["Authorization"];

                List<User> allUsersExceptUser = new();
                User user = getUser(token, connection, logger);

                IDbCommand allUsersCommand = connection.CreateCommand();
                allUsersCommand.CommandText = @"select * from users where token != @token and username != 'admin'";

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
                    logger.Log($"User added to list without current user: {userToAdd.Username}. (Current user: {user.Username})");
                    allUsersExceptUser.Add(userToAdd);
                }

                reader.Close();

                Random rnd = new Random();
                int num = rnd.Next() % allUsersExceptUser.Count;

                User opponent = allUsersExceptUser[num];
                logger.Log($"Opponent for battle chosen: {opponent.Id} - {opponent.Username}. Coins: {opponent.Coins}. Elo: {opponent.Elo}");

                List<Card> userDeck = new();
                List<Card> opponentDeck = new();

                userDeck = getDeck(user, connection, logger);
                logger.Log($"User deck acquired: {userDeck}");
                opponentDeck = getDeck(opponent, connection, logger);
                logger.Log($"Opponent deck acquired: {opponentDeck}");

                if (user == null) {
                    throw new Exception("User not found");
                }

                int i = 1;
                int userWins = 0;
                int userLosses = 0;
                int opponentWins = 0;
                int opponentLosses = 0;
                bool userDeckLast = false;
                bool opponentDeckLast = false;

                while (userDeck.Count > 0 && opponentDeck.Count > 0) {
                    logger.Log($"Beginning round {i}");
                    User winner = fight(userDeck, opponentDeck, user, opponent, connection, logger);
                    logger.Log(winner != null ? $"Round {i} final result: win of {winner.Username}" : $"Round {i} final result: Draw");

                    if (winner == user) {
                        userWins++;
                        opponentLosses++;
                        logger.Log($"User wins, and gets one more win to his account: {userWins}");
                    } else {
                        opponentWins++;
                        userLosses++;
                        logger.Log($"Opponent wins, and gets one more win to his account: {opponentWins}");
                    }

                    // UNIQUE FEATURE - if user has only one card left, the last card's damage is it's basic damage + 5, to make it the last way to defend from loss
                    if (userDeck.Count == 1 && userDeckLast == false) {
                        userDeck[0].Damage += 5;
                        logger.Log($"User {user.Username} has only one card left, damage + 5 to last card, to make defense easier");
                        userDeckLast = true;
                    }

                    if (opponentDeck.Count == 1 && opponentDeckLast == false) {
                        opponentDeck[0].Damage += 5;
                        logger.Log($"Opponent {opponent.Username} has only one card left, damage + 5 to last card, to make defense easier");
                        opponentDeckLast = true;
                    }

                    i++;
                }

                updateStats(user.Username, userWins, userLosses, connection, logger);
                updateStats(opponent.Username, opponentWins, opponentLosses, connection, logger);

                // final steps
                connection.Close();

                string finalWinner = userWins > opponentWins ? user.Username : opponent.Username;
                if (finalWinner == user.Username) {
                    eloUpdate(user, opponent, connection, logger);
                } else {
                    eloUpdate(opponent, user, connection, logger);
                }
                
                logger.Log(userWins > opponentWins ? $"Final winner: {finalWinner} with {userWins} wins." : $"Final winner: {finalWinner} with {opponentWins} wins.");

                rs.ResponseCode = 200;
                rs.ResponseText = "OK";
                rs.ResponseContent = $"Battle finished succesfully - the winner was {finalWinner}";
                rs.ContentType = "text/plain";
                rs.Process();

            }
            catch (Exception) {
                rs.ResponseCode = 400;
                rs.ResponseText = "Bad Request";
                rs.ResponseContent = "Battle unsuccessful";
                rs.ContentType = "text/plain";
                rs.Process();
            }
        }

        private void updateStats(string username, int wins, int losses, IDbConnection connection, Logger logger) {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = @"update users set wins = wins + @wins, losses = losses + @losses where username = @username";

            var pWINS = command.CreateParameter();
            pWINS.DbType = DbType.Int32;
            pWINS.ParameterName = "wins";
            pWINS.Value = wins;
            command.Parameters.Add(pWINS);

            var pLOSSES = command.CreateParameter();
            pLOSSES.DbType = DbType.Int32;
            pLOSSES.ParameterName = "losses";
            pLOSSES.Value = losses;
            command.Parameters.Add(pLOSSES);

            var pUSERNAME = command.CreateParameter();
            pUSERNAME.DbType = DbType.String;
            pUSERNAME.ParameterName = "username";
            pUSERNAME.Value = username;
            command.Parameters.Add(pUSERNAME);

            command.ExecuteNonQuery();
            logger.Log($"User {username} stats updated - +{wins} wins, +{losses} losses.");
        }

        private User getUser(string token, IDbConnection connection, Logger logger) {
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
                logger.Log("User not found");
                throw new Exception("No user found");
            }

            logger.Log($"User {user.Username} found");
            reader.Close();
            return user;
        }

        private User fight(List<Card> userDeck, List<Card> opponentDeck, User user, User opponent, IDbConnection connection, Logger logger) {
            // choose random card from userDeck
            Random rnd = new Random();
            int numUser = rnd.Next() % userDeck.Count;
            Card userCard = userDeck[numUser];
            logger.Log($"User card chosen: {userCard.Name} (card no. {numUser} from deck)");

            // choose random card from opponentDeck
            int numOpponent = rnd.Next() % opponentDeck.Count;
            Card opponentCard = opponentDeck[numOpponent];
            logger.Log($"Opponent card chosen: {opponentCard.Name} (card no. {numOpponent} from deck)");

            User winner = null;
            User loser = null;

            // specialties
            winner = considerSpecial(userCard, opponentCard, user, opponent, logger);

            if (winner != null) {
                loser = winner == user ? opponent : user;
                logger.Log(winner == user ? $"Winner is {winner.Username} because of specialties of his card - {userCard}" : $"Winner is {winner.Username} because of specialties of his card - {opponentCard}");
                winner.Wins++;
                loser.Losses++;
                return winner;
            }
            
            winner = considerNormal(userCard, opponentCard, user, opponent, logger);
            logger.Log(winner == user ? $"Winner is {winner.Username} because of normal attack of his card - {userCard.Name}" : $"Winner is {winner.Username} because of normal attack of his card - {opponentCard.Name}");

            loser = winner == user ? opponent : user;
            
            gamesUpdate(user, opponent, connection, logger);

            if (winner == null) {
                logger.Log($"No winner! Draw!");
                return null;
            } else {
                winner.Wins++;
                loser.Losses++;
            }
            
            if (winner == user) {
                changeOwner(opponentCard, user, connection, logger);
                opponentDeck.Remove(opponentCard);
                logger.Log($"Card {opponentCard.Name} removed from opponent deck after loss");
            } else if (winner == opponent) {
                changeOwner(userCard, opponent, connection, logger);
                userDeck.Remove(userCard);
                logger.Log($"Card {userCard.Name} removed from user deck after loss");
            }

            logger.Log($"Function fight finished, returned {winner}");
            return winner;
        }

        private void changeOwner(Card card, User user, IDbConnection connection, Logger logger) {
            IDbCommand command = connection.CreateCommand();
            command.CommandText = @"update cards set owner_name = @owner_name where id = @id";

            var pOWNER = command.CreateParameter();
            pOWNER.DbType = DbType.String;
            pOWNER.ParameterName = "owner_name";
            pOWNER.Value = user.Username;
            command.Parameters.Add(pOWNER);

            var pID = command.CreateParameter();
            pID.DbType = DbType.String;
            pID.ParameterName = "id";
            pID.Value = card.Id;
            command.Parameters.Add(pID);

            command.Prepare();
            command.ExecuteNonQuery();
            logger.Log($"Card {card.Name} changed owner to {user.Username}");
        }

        private User considerNormal(Card userCard, Card opponentCard, User user, User opponent, Logger logger) {
            if ((userCard.Element == Model.Enums.Element.Water && opponentCard.Element == Model.Enums.Element.Fire) ||
                (userCard.Element == Model.Enums.Element.Fire && opponentCard.Element == Model.Enums.Element.Water)) {
                logger.Log(userCard.Element == Model.Enums.Element.Water ? "EFFECTIVE ATTACK! User's water beats opponent's fire" : "EFFECTIVE ATTACK! Opponent's water beats user's fire");
                return userCard.Element == Model.Enums.Element.Water ? user : opponent;
            }
            if ((userCard.Element == Model.Enums.Element.Fire && opponentCard.Element == Model.Enums.Element.Neutral) ||
                (userCard.Element == Model.Enums.Element.Neutral && opponentCard.Element == Model.Enums.Element.Fire)) {
                logger.Log(userCard.Element == Model.Enums.Element.Fire ? "EFFECTIVE ATTACK! User's fire beats opponent's non-elementary card" : "EFFECTIVE ATTACK! Opponent's fire beats user's non-elementary card");
                return userCard.Element == Model.Enums.Element.Fire ? user : opponent;
            }
            if ((userCard.Element == Model.Enums.Element.Neutral && opponentCard.Element == Model.Enums.Element.Water) ||
                userCard.Element == Model.Enums.Element.Water && opponentCard.Element == Model.Enums.Element.Neutral) {
                logger.Log(userCard.Element == Model.Enums.Element.Neutral ? "EFFECTIVE ATTACK! User's non-elementary card beats opponent's water" : "EFFECTIVE ATTACK! Opponent's non-elementary card beats user's water");
                return userCard.Element == Model.Enums.Element.Neutral ? user : opponent;
            }

            return userCard.Damage > opponentCard.Damage ? user : opponent;
        }

        private void gamesUpdate(User user, User opponent, IDbConnection connection, Logger logger) {
            user.Games++;
            opponent.Games++;

            gamesCommandUpdate(user, connection, logger);
            gamesCommandUpdate(opponent, connection, logger);
            logger.Log($"User has now: {user.Games} games, opponent: {opponent.Games} games");
        }
        
        private void gamesCommandUpdate(User user, IDbConnection connection, Logger logger) {
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
            logger.Log($"Games updated for {user.Username}");
        }

        private void eloUpdate(User winner, User loser, IDbConnection connection, Logger logger) {
            int winsWinner = winner.Wins;
            int lossesWinner = winner.Losses;
            int gamesWinner = winner.Games;

            int winsLoser = loser.Wins;
            int lossesLoser = loser.Losses;
            int gamesLoser = loser.Games;

            int eloWinner = winner.Elo;
            int eloLoser = loser.Elo;

            int eloOpponentWinner = getOpponentElo(winner, connection, logger);
            int eloOpponentLoser = getOpponentElo(loser, connection, logger);

            int eloWinnerNew = calculateElo(eloOpponentWinner, winsWinner, lossesWinner, gamesWinner, logger);
            logger.Log($"Elo of winner {winner.Username} will be changed from {eloWinner} to {eloWinnerNew}");
            int eloLoserNew = calculateElo(eloOpponentLoser, winsLoser, lossesLoser, gamesLoser, logger);
            logger.Log($"Elo of loser {loser.Username} will be changed from {eloLoser} to {eloLoserNew}");

            eloCommandUpdate(winner, eloWinnerNew, connection, logger);
            logger.Log($"Elo of winner {winner.Username} changed.");
            eloCommandUpdate(loser, eloLoserNew, connection, logger);
            logger.Log($"Elo of loser {loser.Username} changed.");
        }

        private void eloCommandUpdate(User user, int eloNew, IDbConnection connection, Logger logger) {
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

        private int calculateElo(int opponentRating, int wins, int losses, int games, Logger logger) {
            int up = opponentRating + (400 * (wins - losses));
            int elo = up / games;
            return elo < 100 ? 100 : elo;
        }

        private int getOpponentElo(User user, IDbConnection connection, Logger logger) {
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

        private User considerSpecial(Card userCard, Card opponentCard, User user, User opponent, Logger logger) {
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

        private List<Card> getDeck(User user, IDbConnection connection, Logger logger) {
            List<Card> allCards = new();
            List<Card> chosenDeck = new();
            
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
                    chosenDeck.Add(toAdd);
                } else {
                    allCards.Add(toAdd);
                }
            }

            if (allCards.Count == 0) {
                throw new Exception("No cards found");
            }

            while (chosenDeck.Count < 4) {
                chosenDeck.Add(allCards[0]);
                allCards.RemoveAt(0);
            }

            if (chosenDeck.Count > 4) {
                throw new Exception("Deck is too big");
            }

            reader.Close();
            
            return chosenDeck;
        }
    }
}
