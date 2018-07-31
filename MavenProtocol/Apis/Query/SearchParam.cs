namespace MavenProtocol.Apis
{
    //https://search.maven.org/#api
    //http://search.maven.org/solrsearch/select?q=g:%22com.google.inject%22&rows=20&wt=json
    //  http://central.maven.org/maven2/com/google/inject/guice-bom/4.2.0/
    //  http://central.maven.org/maven2/com/google/inject/guice/4.2.0/
    public class SearchParam
    {
        public string FreeText { get; set; }
        public string Query { get; set; }
        public string Wt { get; set; }//always json
        public int Rows { get; set; }//take
        public int Start { get; set; }//skip
    }
}
