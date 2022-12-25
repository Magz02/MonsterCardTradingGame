namespace MonsterTradingCardGame.BL.HTTP {
    public class HttpResponse {
        private StreamWriter writer;
        public int ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public Dictionary<string, string> headers = new();
        public string ResponseContent { get; set; }
        public List<string> ResponseContentList { get; set; }
        public string ContentType {
            get {
                return headers["Content-Type"];
            }
            set {
                headers["Content-Type"] = value;
            }
        }
        
        public HttpResponse(StreamWriter writer) {
            this.writer = writer;
        }

        public void Process() {
            string msg = $"HTTP/1.1 {ResponseCode} {ResponseText}";
            Console.WriteLine(msg);
            writer.WriteLine(msg);

            if (ContentType == "application/json") {
                if (ResponseContent != null) {
                    writer.WriteLine($"Content: {ResponseContent}");
                }
                // TODO: Headers
                if (ResponseContent != null && ResponseContent.Length > 0) {
                    headers.Add("Content-Length", ContentType.Length.ToString());
                }
                foreach (var kvp in headers) {
                    writer.WriteLine($"{kvp.Key}: {kvp.Value}");
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            } else if (ContentType == "text/plain") {
                foreach (var line in ResponseContentList) {
                    writer.WriteLine(line);
                    Console.WriteLine(line);
                }
            }


            writer.WriteLine();
            Console.WriteLine();
            // Content print
            if (ResponseContent != null && ResponseContent.Length > 0) {
                writer.WriteLine(ResponseContent);
                Console.WriteLine(ResponseContent);
            }
            //writer.WriteLine(ResponseContent);
            writer.Flush();
            writer.Close();
        }
    }
}