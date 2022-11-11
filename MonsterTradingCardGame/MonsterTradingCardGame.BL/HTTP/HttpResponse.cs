namespace MonsterTradingCardGame.BL.HTTP {
    public class HttpResponse {
        private StreamWriter writer;
        public int ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public Dictionary<string, string> Headers { get; }
        public string ResponseContent { get; set; }
        
        public HttpResponse(StreamWriter writer) {
            this.writer = writer;
            Headers = new Dictionary<string, string>();
        }

        public void Process() {
            string msg = $"HTTP/1.1 {ResponseCode} {ResponseText}";
            writer.WriteLine(msg);
            // headers... (skipped)
            writer.WriteLine();
            writer.WriteLine(ResponseContent);
            writer.Flush();
            writer.Close();
        }
    }
}