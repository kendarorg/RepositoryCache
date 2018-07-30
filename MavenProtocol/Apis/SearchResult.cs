using Newtonsoft.Json;

namespace MavenProtocol.Apis
{
    public class SearchResult
    {
        public SearchResult(ResponseHeader header,ResponseContent content)
        {
            Header = header;
            Content = content;
        }
        [JsonProperty("responseHeader")]
        public ResponseHeader Header { get; set; }
        [JsonProperty("response")]
        public ResponseContent Content { get; set; }
    }
}