﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL.HTTP {
    public interface IHttpEndpoint {
        void HandleRequest(HttpRequest rq, HttpResponse rs);
    }
}
