using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Model.Logger {
    public class Logger : ILogger {

        public Logger() {
            
        }

        public void Log(string message) {
            string timestamp = getTimestamp(DateTime.Now);
            Console.WriteLine($"{timestamp}: {message}");
        }

        public string getTimestamp(DateTime value) {
            return value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
