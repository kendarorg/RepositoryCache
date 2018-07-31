using System.Collections.Generic;

namespace MavenProtocol.Apis.Browse
{
    public class ExploreResult
    {
        public byte[] Content { get; set; }
        public string Base { get; set; }
        public List<string> Children { get; set; }
    }
}