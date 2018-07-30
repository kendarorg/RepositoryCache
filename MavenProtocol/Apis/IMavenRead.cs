using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol.Apis
{
    public interface IMavenRead
    {
        byte[] ReadPackage(Guid repoId, string path, string packageId,string subType);
        string ReadPom(Guid repoId, string path, string packageId);
        string ReadMetadata(Guid repoId, string path, string packageId);
    }
}
