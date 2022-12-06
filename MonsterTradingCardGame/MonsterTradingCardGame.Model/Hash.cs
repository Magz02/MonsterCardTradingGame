using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Model {
    public class Hash {

        public Hash() {
            
        }
        
        public string HashValue(string password) {
            var sha = SHA256.Create();

            var asByteArray = Encoding.Default.GetBytes(password);
            var hashedPassword = sha.ComputeHash(asByteArray);
            
            return Convert.ToBase64String(hashedPassword);
        }
    }
}
