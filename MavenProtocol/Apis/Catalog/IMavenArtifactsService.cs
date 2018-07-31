using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol.Apis
{
    public interface IMavenArtifactsService
    {
        /// <summary>
        /// /{repo}/group/artifactid/version/artifactid-version.jar
        /// </summary>
        /// <param name="repoId"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        byte[] ReadArtifact(Guid repoId, MavenIndex item);
        string ReadChecksum(Guid repoId, MavenIndex item);
        void WriteArtifact(Guid repoId, MavenIndex item,byte[] content);
        void WriteChecksum(Guid repoId, MavenIndex item,string content);
        void DeleteArtifact(Guid repoId, MavenIndex);
    }
}
