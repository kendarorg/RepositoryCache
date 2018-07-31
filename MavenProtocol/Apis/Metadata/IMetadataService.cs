using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol.Apis.Catalog
{
    public interface IMetadataService
    {
        MavenMetadata ReadMetadata(Guid repoId, MavenIndex item);
        string ReadChecksum(Guid repoId, MavenIndex item);
        void WriteMetadata(Guid repoId, MavenIndex item, MavenMetadata metadata);
        void WriteChecksum(Guid repoId, MavenIndex item, string content);
    }
}
