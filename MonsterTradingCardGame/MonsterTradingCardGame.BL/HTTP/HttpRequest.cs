using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame.BL.HTTP {
    public class HttpRequest {
        private StreamReader reader;
        public HttpMethod Method { get; set; }
        public string[] Path { get; private set; }
        public string ProtocolVersion { get; private set; }
        public Dictionary<string, string> QueryParams = new();
        public Dictionary<string, string> headers = new();
        public string Content { get; private set; }

        public HttpRequest(StreamReader reader) {
            this.reader = reader;
        }

        public void Parse() {
            // first line contains HTTP METHOD PATH and PROTOCOL
            string line = reader.ReadLine();
            Console.WriteLine($"Line: {line}");
            var firstLineParts = line.Split(' ');
            Console.WriteLine($"FirstLineParts0: {firstLineParts[0]}");
            Console.WriteLine($"FirstLineParts1: {firstLineParts[1]}");
            Console.WriteLine($"FirstLineParts2: {firstLineParts[2]}");
            Method = (HttpMethod)Enum.Parse(typeof(HttpMethod), firstLineParts[0]);
            Console.WriteLine($"Method: {Method}");
            Path = firstLineParts[1].Split('?');
            Console.WriteLine($"Path0: {Path[0]}");
            var pathParts = Path[0].Split("?");
            Console.WriteLine($"PathParts0: {pathParts[0]}");
            if (Path.Length == 2) {
                // we have query params after the ?-char
                var queryParams = Path[1].Split('&');
                foreach (string queryParam in queryParams) {
                    var queryParamParts = queryParam.Split('=');
                    if (queryParamParts.Length == 2) {
                        QueryParams.Add(queryParamParts[0], queryParamParts[1]);
                    } else {
                        QueryParams.Add(queryParamParts[0], null);
                    }
                }
            }
            Path = pathParts[0].Split('/');

            ProtocolVersion = firstLineParts[2];

            // headers
            while ((line = reader.ReadLine()) != null) {
                Console.WriteLine(line);
                if (line.Length == 0) {
                    break;
                }

                var headerParts = line.Split(": ");
                headers.Add(headerParts[0], headerParts[1]);
            }

            // content
            Content = "";
            var data = new StringBuilder();
            if (headers.ContainsKey("Content-Length")) {
                int contentLength = int.Parse(headers["Content-Length"]);
                if (contentLength > 0) {
                    char[] buffer = new char[1024];
                    int totalBytesRead = 0;
                    while (totalBytesRead < contentLength) {
                        var bytesRead = reader.Read(buffer, 0, 1024);
                        if (bytesRead == 0)
                            break;
                        totalBytesRead += bytesRead;
                        data.Append(buffer, 0, bytesRead);
                    }
                }
                Content = data.ToString();
            }
        }
    }
}
