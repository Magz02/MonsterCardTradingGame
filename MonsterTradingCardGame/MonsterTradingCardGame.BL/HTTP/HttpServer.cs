using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MonsterTradingCardGame.BL.HTTP {
    public class HttpServer {
        private readonly int port;
        private readonly IPAddress loopback;

        private TcpListener httpServer;

        public HttpServer(IPAddress adr, int port) {
            this.loopback = adr;
            this.port = port;
        }

        public void Run() {
            httpServer = new TcpListener(loopback, port);
            httpServer.Start();
            while (true) {
                Console.WriteLine("Waiting for connection...");
                var clientSocket = httpServer.AcceptTcpClient();
                var httpProcessor = new HttpProcessor(clientSocket);
                Task.Factory.StartNew(() => httpProcessor.run());
            }
        }
    }
}
