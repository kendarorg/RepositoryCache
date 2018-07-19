namespace NugetProtocol
{
    public class QueryVersion
    {
        public QueryVersion()
        {

        }
        public QueryVersion(string oid, string version, int downloads = 0)
        {
            OId = oid;
            Version = version;
            Downloads = downloads;
        }

        public string OId { get; set; }
        public string Version { get; set; }
        public int Downloads { get; set; }
    }
}
