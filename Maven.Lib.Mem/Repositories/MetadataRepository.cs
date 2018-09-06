using Maven.News;
using MultiRepositories;
using Repositories;
using System;
using System.Collections.Generic;

namespace Maven.Repositories
{
    public class MetadataRepository : InMemoryRepository<MetadataEntity>, IMetadataRepository
    {
        public MetadataRepository(AppProperties properties) : base(properties)
        {
        }

        public MetadataEntity GetArtifactMetadata(Guid repoId, string[] group, string artifactId, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public MetadataEntity GetArtifactVersionMetadata(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MetadataEntity> GetVersions(Guid repoId, string[] group, string artifactId, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }
    }
}
