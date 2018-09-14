﻿using Maven.Services;
using MavenProtocol;
using MavenProtocol.Apis;
using MavenProtocol.News;
using MultiRepositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maven.News
{
    public class ExploreApi : IExploreApi
    {
        private readonly IServicesMapper _servicesMapper;
        private readonly IArtifactsStorage _artifactsStorage;
        private readonly IRepositoryEntitiesRepository _repositoriesRepository;
        private readonly IMetadataRepository _metadataRepository;
        private readonly IArtifactsRepository _artifactsRepository;
        private readonly IReleaseArtifactRepository _releaseArtifactRepository;
        private readonly IPomRepository _pomRepository;

        public ExploreApi(IServicesMapper servicesMapper, IArtifactsStorage artifactsStorage,
            IRepositoryEntitiesRepository repositoriesRepository,
            IMetadataRepository metadataRepository, 
            IArtifactsRepository artifactsRepository,
            IReleaseArtifactRepository releaseArtifactRepository,IPomRepository pomRepository)
        {
            this._servicesMapper = servicesMapper;
            this._artifactsStorage = artifactsStorage;
            this._repositoriesRepository = repositoriesRepository;
            this._metadataRepository = metadataRepository;
            this._artifactsRepository = artifactsRepository;
            this._releaseArtifactRepository = releaseArtifactRepository;
            this._pomRepository = pomRepository;
        }
        public ExploreResponse Retrieve(MavenIndex mi)
        {
            var result = new ExploreResponse
            {
                Children = new List<string>()
            };
            var repo = _repositoriesRepository.GetById(mi.RepoId);
            foreach (var dir in _artifactsStorage.GetSubDir(repo, mi))
            {
                result.Children.Add(dir);
            }

            if (!string.IsNullOrWhiteSpace(mi.Version) && !string.IsNullOrWhiteSpace(mi.ArtifactId))
            {
                AddMetadata(result);
                var poms = new HashSet<string>();
                var timestampedSnapshot = _servicesMapper.HasTimestampedSnapshot(mi.RepoId);
                foreach (var item in _artifactsRepository.GetAllArtifacts(mi.RepoId, mi.Group, mi.ArtifactId, mi.Version, mi.IsSnapshot))
                {
                    if (timestampedSnapshot)
                    {
                        var classi = string.IsNullOrWhiteSpace(item.Classifier) ? "" : "-" + item.Classifier;
                        var build = string.IsNullOrWhiteSpace(item.Build) ? "" : "-" + item.Timestamp.ToString("yyyyMMdd.HHmmss") + "-" + item.Build;
                        mi.Build = item.Build;
                        mi.Timestamp = item.Timestamp;
                        var name = item.ArtifactId + "-" + item.Version + build + classi + "." + item.Extension;
                        result.Children.Add(name);
                        result.Children.Add(name + ".md5");
                        result.Children.Add(name + ".sha1");
                    }
                    else
                    {
                        var classi = string.IsNullOrWhiteSpace(item.Classifier) ? "" : "-" + item.Classifier;
                        var name = item.ArtifactId + "-" + BuildFullVersion(item.Version, item.IsSnapshot) + classi + "." + item.Extension;
                        result.Children.Add(name);
                        result.Children.Add(name + ".md5");
                        result.Children.Add(name + ".sha1");
                    }

                }
                AddPom(result,mi);
            }
            else if (string.IsNullOrWhiteSpace(mi.Version))
            {
                var meta = _metadataRepository.GetArtifactMetadata(mi.RepoId, mi.Group, mi.ArtifactId);
                if (meta != null)
                {
                    AddMetadata(result);
                    foreach (var item in _metadataRepository.GetVersions(mi.RepoId, mi.Group, mi.ArtifactId))
                    {
                        var ver = BuildFullVersion(item.Version, item.IsSnapshot);
                        if (!result.Children.Contains(ver))
                        {
                            result.Children.Add(ver);
                        }
                    }
                }
            }

            return result;
        }

        private void AddPom(ExploreResponse result,MavenIndex mi)
        {
            foreach (var sipom in _pomRepository.GetPomForVersion(mi.RepoId, mi.Group, mi.ArtifactId, mi.Version, mi.IsSnapshot))
            {
                var build = string.IsNullOrWhiteSpace(sipom.Build) ? "" : "-" + sipom.Timestamp.ToString("yyyyMMdd.HHmmss") + "-" + sipom.Build;
                var name = sipom.ArtifactId + "-" + sipom.Version + build;
                result.Children.Add(name + ".pom");
                result.Children.Add(name + ".pom.md5");
                result.Children.Add(name + ".pom.sha1");
            }
        }

        private string BuildFullVersion(string version, bool isSnapshot)
        {
            return version + (isSnapshot ? "-SNAPSHOT" : "");
        }

        private static void AddMetadata(ExploreResponse result)
        {
            result.Children.Add("maven-metadata.xml");
            result.Children.Add("maven-metadata.xml.md5");
            result.Children.Add("maven-metadata.xml.sha1");
        }
    }
}
