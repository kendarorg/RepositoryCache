using Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol.Apis
{
    public interface IArtifactsService:ISingleton
    {
        void SetArtifactChecksums(Guid id, MavenIndex idx, string content);
        void UploadArtifact(Guid id, MavenIndex idx, byte[] content);
        void SetMetadataChecksums(Guid id, MavenIndex idx, string content);
        void UpdateMetadata(Guid id, MavenIndex idx, string content);
        void SetTags(Guid id, MavenIndex idx, string[] tags);
    }
}
