using MavenProtocol.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol.Test
{
    public class SampleMavenRead : IMavenRead
    {
        public string ReadMetadata(Guid repoId, string path, string packageId)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadPackage(Guid repoId, string path, string packageId)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadPackage(Guid repoId, string path, string packageId, string subType)
        {
            throw new NotImplementedException();
        }

        public string ReadPackageMd5(Guid repoId, string path, string packageId)
        {
            throw new NotImplementedException();
        }

        public string ReadPackageSha(Guid repoId, string path, string packageId)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadPom(Guid repoId, string path, string packageId)
        {
            throw new NotImplementedException();
        }

        public string ReadPomMd5(Guid repoId, string path, string packageId)
        {
            throw new NotImplementedException();
        }

        public string ReadPomSha(Guid repoId, string path, string packageId)
        {
            throw new NotImplementedException();
        }

        string IMavenRead.ReadPom(Guid repoId, string path, string packageId)
        {
            throw new NotImplementedException();
        }
    }
}
