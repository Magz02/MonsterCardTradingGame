using MonsterTradingCardGame.BL.BM;
using MonsterTradingCardGame.BL.CM;
using MonsterTradingCardGame.BL.SM;
using MonsterTradingCardGame.BL.UM;
using System.Net.Sockets;

namespace MonsterTradingCardGame.BL.HTTP {
    public class HttpProcessor {
        private TcpClient clientSocket;

        public HttpProcessor(TcpClient clientSocket) {
            this.clientSocket = clientSocket;
        }
        
        public void run() {
            var reader = new StreamReader(clientSocket.GetStream());
            var request = new HttpRequest(reader);
            request.Parse();

            var writer = new StreamWriter(clientSocket.GetStream()) { AutoFlush = true };
            var response = new HttpResponse(writer);

            if (request.Path[1].Equals("users")) {
                if (request.Method == HttpMethod.POST) {
                    RegisterUserEndpoint userController = new RegisterUserEndpoint();
                    userController.HandleRequest(request, response);
                } else if (request.Method == HttpMethod.GET && request.Path[2] != null) {
                    GetUserEndpoint userController = new GetUserEndpoint();
                    userController.HandleRequest(request, response);
                } else if (request.Method == HttpMethod.PUT && request.Path[2] != null) {
                    EditUserEndpoint userController = new EditUserEndpoint();
                    userController.HandleRequest(request, response);
                } else {
                    response.ResponseCode = 400;
                    response.ResponseText = "Bad Request";
                    response.ResponseContent = "Not enough arguments or wrong arguments given";
                    response.Process();
                }
            } else if (request.Path[1].Equals("sessions")) {
                LoginUserEndpoint userController = new LoginUserEndpoint();
                userController.HandleRequest(request, response);
            } else if (request.Path[1].Equals("packages")) {
                // creates packages of cards
                CreatePackageEndpoint packageController = new CreatePackageEndpoint();
                packageController.HandleRequest(request, response);
            } else if (request.Path[1].Equals("transactions") && request.Path[2].Equals("packages")) {
                AcquirePackageEndpoint packageController = new AcquirePackageEndpoint();
                packageController.HandleRequest(request, response);
            } else if (request.Path[1].Equals("cards")) {
                AcquireCardsEndpoint cardController = new AcquireCardsEndpoint();
                cardController.HandleRequest(request, response);
            } else if (request.Path[1].Equals("deck")) {
                if (request.Method == HttpMethod.PUT) {
                    EditDeckEndpoint deckController = new EditDeckEndpoint();
                    deckController.HandleRequest(request, response);
                } else if (request.Method == HttpMethod.GET) {
                    AcquireDeckEndpoint deckController = new AcquireDeckEndpoint();
                    deckController.HandleRequest(request, response);
                } else {
                    response.ResponseCode = 400;
                    response.ResponseText = "Bad Request";
                    response.ResponseContent = "Not enough arguments or wrong arguments given";
                    response.Process();
                }
            } else if (request.Path[1].Equals("stats")) {
                StatisticsEndpoint statsController = new StatisticsEndpoint();
                statsController.HandleRequest(request, response);
            } else if (request.Path[1].Equals("score")) {
                ScoreboardEndpoint scoreController = new ScoreboardEndpoint();
                scoreController.HandleRequest(request, response);
            } else if (request.Path[1].Equals("battles")) {
                BattleEndpoint battleController = new BattleEndpoint();
                battleController.HandleRequest(request, response);
            } else if (request.Path[1].Equals("logout")) {
                LogoutUserEndpoint logoutEndpoint = new LogoutUserEndpoint();
                logoutEndpoint.HandleRequest(request, response);
            } else {
                response.ResponseCode = 200;
                response.ResponseText = "OK";
                response.headers.Add("Content-Length", response.ResponseContent.Length.ToString());
                response.headers.Add("Content-Type", "text/plain");
                response.Process();
            }
        }
    }
}
