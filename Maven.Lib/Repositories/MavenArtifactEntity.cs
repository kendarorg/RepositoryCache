using Repositories;

namespace Maven.Repositories
{
    public class MavenArtifactEntity : BaseEntity
    {
        public string Md5 { get; set; }
        public string Sha1 { get; set; }
        public string Asc { get; set; }
    }
}