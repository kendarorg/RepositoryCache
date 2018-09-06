using Ioc;
using Maven.News;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MavenProtocol.News
{
    public interface IDummyGenerator:ISingleton
    {
        MetadataEntity GetArtifactMetadata(string[] group, string artifactId, ITransaction transaction = null);
        MetadataEntity GetArtifactVersionMetadata(string[] group, string artifactId, string version, bool isSnapshot, string build);
    }
}
