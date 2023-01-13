using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.Model.Logger {
    public interface ILogger {
        public void Log(string message);
        public string getTimestamp(DateTime value);
    }
}
