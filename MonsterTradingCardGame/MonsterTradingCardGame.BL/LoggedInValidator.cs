using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL {
    public class LoggedInValidator {
        public LoggedInValidator() {

        }

        public bool Validate(Dictionary<string, string> headers, IDbConnection connection) {
            int i = 0;
            
            if (headers.ContainsKey("Authorization")) {
                string token = headers["Authorization"];

                IDbCommand command = connection.CreateCommand();
                command.CommandText = @"select * from users where token = @token";

                var pTOKEN = command.CreateParameter();
                pTOKEN.ParameterName = "token";
                pTOKEN.DbType = DbType.String;
                pTOKEN.Value = headers["Authorization"];
                command.Parameters.Add(pTOKEN);

                var reader = command.ExecuteReader();
                if (reader.Read()) {
                    i++;
                }
            }
            
            return i == 1 ? true : false;
        }

        public bool ValidateAdmin(Dictionary<string, string> headers, IDbConnection connection) {
            int i = 0;
            string name = "";
            IDbCommand command = connection.CreateCommand();
            command.CommandText = @"select username from users where token = @token";

            var pTOKEN = command.CreateParameter();
            pTOKEN.ParameterName = "token";
            pTOKEN.DbType = DbType.String;
            pTOKEN.Value = headers["Authorization"];
            command.Parameters.Add(pTOKEN);

            var reader = command.ExecuteReader();
            while (reader.Read()) {
                name = reader.GetString(0);
                i++;
            }

            return i == 1 && name == "admin" ? true : false;

        }
    }
}
