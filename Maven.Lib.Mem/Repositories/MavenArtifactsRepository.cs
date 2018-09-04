﻿using MultiRepositories;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.Repositories
{
    public class MavenArtifactsRepository : InMemoryRepository<OLDMavenArtifactEntity>, OLDIMavenArtifactsRepository
    {
        public MavenArtifactsRepository(AppProperties properties) : base(properties)
        {
        }

        public OLDMavenArtifactEntity GetById(Guid repoId, string[] group, string artifactId, string version, string classifier)
        {
            return GetAll().Where(a => a.RepositoryId == repoId).
                FirstOrDefault(a => a.ArtifactId == artifactId &&
                    a.GroupId == string.Join(".", group) &&
                    a.Version == version && a.Classifier == classifier);
        }

        public IEnumerable<OLDMavenArtifactEntity> GetById(Guid repoId, string[] group, string artifactId, string version)
        {
            return GetAll().Where(a => a.RepositoryId == repoId).
                Where(a => a.ArtifactId == artifactId &&
                    a.GroupId == string.Join(".", group) &&
                    a.Version == version);
        }

        public IEnumerable<OLDMavenArtifactEntity> GetVersionsByIdFirstIsRelease(Guid repoId, string[] group, string artifactId)
        {
            throw new NotImplementedException();
        }
    }
}