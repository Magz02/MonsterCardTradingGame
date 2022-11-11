using MonsterTradingCardGame.BL.CM;
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
            // TODO: Communication to BL

            if (request.Path[1].Equals("users") && request.Method == HttpMethod.POST) {
                if (request.Method == HttpMethod.POST) {
                    RegisterUserEndpoint userController = new RegisterUserEndpoint();
                    userController.HandleRequest(request, response);
                } else if (request.Method == HttpMethod.GET) {
                    // TODO: Get user - CURL 14
                } else if (request.Method == HttpMethod.PUT) {
                    // TODO: Edit user - CURL 14
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
                writer.WriteLine("Creating packages");
                CreatePackageEndpoint packageController = new CreatePackageEndpoint();
                packageController.HandleRequest(request, response);
            } else if (request.Path[1].Equals("transactions") && request.Path[2].Equals("packages")) {
                writer.WriteLine("Acquiring packages...");
                AcquirePackageEndpoint packageController = new AcquirePackageEndpoint();
                packageController.HandleRequest(request, response);
            } else if (request.Path[1].Equals("cards")) {
                // TODO: Show all acquired cards CURL 8-9
                writer.WriteLine("Acquiring cards...");
            } else if (request.Path[1].Equals("deck")) {
                if (request.Method == HttpMethod.PUT) {
                    // TODO: Show and configure decks - CURL 11
                } else if (request.Method == HttpMethod.GET) {
                    if (request.QueryParams != null) {
                        // TODO: Show different representation - CURL 13
                    } else {
                        // TODO: Show - CURL 10-12
                    }
                } else {
                    response.ResponseCode = 400;
                    response.ResponseText = "Bad Request";
                    response.ResponseContent = "Not enough arguments or wrong arguments given";
                    response.Process();
                }
            } else {
                Thread.Sleep(10000);
                writer.WriteLine("This is interesting...");
                response.ResponseCode = 200;
                response.ResponseText = "OK";
                response.Headers.Add("Content-Length", response.ResponseContent.Length.ToString());
                response.Headers.Add("Content-Type", "application/json");
                // TODO: Refactor response.ResponseContent
                response.Process();
            }
        }
    }
}