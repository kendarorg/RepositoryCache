namespace NugetProtocol
{
    public class QueryContext
    {
        public QueryContext()
        {

        }
        public QueryContext(string ovocab, string obase)
        {
            OVocab = ovocab;
            OBase = obase;
        }
        public string OVocab { get; set; }
        public string OBase { get; set; }
    }
}
