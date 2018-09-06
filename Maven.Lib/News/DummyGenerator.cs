using MavenProtocol.News;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.News
{
    public class DummyGenerator : IDummyGenerator
    {
        private IMetadataRepository _metadataRepository;

        public DummyGenerator(IMetadataRepository metadataRepository)
        {
            this._metadataRepository = metadataRepository;
        }

        public MetadataEntity GetArtifactMetadata(string[] group, string artifactId,ITransaction transaction=null)
        {
            var entity = _metadataRepository.GetArtifactMetadata(group, artifactId, transaction);
            if (entity == null)
            {
                entity = new MetadataEntity
                {
                    ArtifactId = artifactId,
                    Group = string.Join(".", group),
                    Initialized = false
                };
            }
            return entity;
        }
    }
}
