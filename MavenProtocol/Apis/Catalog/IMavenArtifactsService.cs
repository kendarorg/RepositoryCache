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
        byte[] ReadPackage(Guid repoId, MavenIndex item);
        byte[] ReadPom(Guid repoId, MavenIndex item);
        string ReadChecksum(Guid repoId, MavenIndex item);
        void WritePackage(Guid repoId, MavenIndex item,byte[] content);
        void WritePom(Guid repoId, MavenIndex item,byte[] content);
        void WriteChecksum(Guid repoId, MavenIndex item,string content);
    }
}
