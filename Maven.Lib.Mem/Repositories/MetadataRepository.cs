using Maven.News;
using MultiRepositories;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maven.Repositories
{
    public class MetadataRepository : InMemoryRepository<MetadataEntity>, IMetadataRepository
    {
        public MetadataRepository(AppProperties properties) : base(properties)
        {
        }

        public MetadataEntity GetArtifactMetadata(Guid repoId, string[] group, string artifactId, ITransaction transaction = null)
        {
            return GetAll().FirstOrDefault(a =>
            {
                return a.RepositoryId == repoId && a.Group == string.Join(".", group) && 
                a.ArtifactId == artifactId && a.Version==null;
            }
                );
        }

        public MetadataEntity GetArtifactVersionMetadata(Guid repoId, string[] group, string artifactId, string version, bool isSnapshot, ITransaction transaction = null)
        {
            return GetAll().FirstOrDefault(a =>
            {
                return a.RepositoryId == repoId && a.Group == string.Join(".", group) && a.ArtifactId == artifactId
                && a.Version == version && a.IsSnapshot == isSnapshot;
            }
                );
        }

        public IEnumerable<MetadataEntity> GetVersions(Guid repoId, string[] group, string artifactId, ITransaction transaction = null)
        {
            return GetAll().Where(a =>
            {
                return a.RepositoryId == repoId && a.Group == string.Join(".", group) && a.ArtifactId == artifactId && !string.IsNullOrWhiteSpace(a.Version);
            }
                );
        }
    }
}
