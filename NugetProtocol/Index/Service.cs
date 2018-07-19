namespace NugetProtocol
{
    public class Service
    {
        public Service()
        {

        }
        public Service(string oid,string otype,string comment)
        {
            OId = oid;
            OType = otype;
            Comment = comment;
        }

        public string OId { get; set; }
        public string OType { get; set; }
        public string Comment { get; set; }
    }
}
