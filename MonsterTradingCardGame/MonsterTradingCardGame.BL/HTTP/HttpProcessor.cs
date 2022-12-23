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

        // TODO (Refactor): Method checking to be put in Endpoint classes instead of this class
        // Refactor concerns users, deck and tradings!!!
        public void run() {
            var reader = new StreamReader(clientSocket.GetStream());
            var request = new HttpRequest(reader);
            request.Parse();

            var writer = new StreamWriter(clientSocket.GetStream()) { AutoFlush = true };
            var response = new HttpResponse(writer);

            if (request.Path[1].Equals("users") && request.Method == HttpMethod.POST) {
                if (request.Method == HttpMethod.POST) {
                    RegisterUserEndpoint userController = new RegisterUserEndpoint();
                    userController.HandleRequest(request, response);
                } else if (request.Method == HttpMethod.GET) {
                    GetUserEndpoint userController = new GetUserEndpoint();
                    userController.HandleRequest(request, response);
                } else if (request.Method == HttpMethod.PUT) {
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
                    // TODO: Show and configure decks - CURL 11
                    writer.WriteLine("Configuring decks...");
                    EditDeckEndpoint deckController = new EditDeckEndpoint();
                    //deckController.HandleRequest(request, response);
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
                // TODO: Show stats - CURL 15, 18
                writer.WriteLine("Showing stats...");
                StatisticsEndpoint statsController = new StatisticsEndpoint();
                //statsController.HandleRequest(request, response);
            } else if (request.Path[1].Equals("score")) {
                // TODO: Show scoreboard - CURL 16, 19
                writer.WriteLine("Showing scoreboard...");
                ScoreboardEndpoint scoreController = new ScoreboardEndpoint();
                //scoreController.HandleRequest(request, response);
            } else if (request.Path[1].Equals("battle")) {
                // TODO: Battle - CURL 17
                writer.WriteLine("Starting battle...");
                BattleEndpoint battleController = new BattleEndpoint();
                //battleController.HandleRequest(request, response);
            } else if (request.Path[1].Equals("tradings")) {
                // TODO: Trading deals - GET, POST, DELETE, 20-21
                writer.WriteLine("Trading deals...");
                if (request.Method == HttpMethod.POST) {
                    TradeEndpoint tradeController = new TradeEndpoint();
                    //tradeController.HandleRequest(request, response);
                } else if (request.Method == HttpMethod.GET) {
                    // Refactor for method in endpoint
                } else if (request.Method == HttpMethod.DELETE) {
                    
                } else {
                    response.ResponseCode = 400;
                    response.ResponseText = "Bad Request";
                    response.ResponseContent = "Not enough arguments or wrong argument given";
                    response.Process();
                }
                
            } else {
                Thread.Sleep(10000);
                writer.WriteLine("This is interesting...");
                response.ResponseCode = 200;
                response.ResponseText = "OK";
                response.headers.Add("Content-Length", response.ResponseContent.Length.ToString());
                response.headers.Add("Content-Type", "text/plain");
                // TODO: Refactor response.ResponseContent
                response.Process();
            }
        }
    }
}