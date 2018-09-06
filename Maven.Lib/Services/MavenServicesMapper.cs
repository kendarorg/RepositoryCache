using Ioc;
using MavenProtocol;
using MavenProtocol.Apis;
using MultiRepositories;
using MultiRepositories.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Maven
{
    public class MavenServicesMapper : IServicesMapper, ISingleton
    {
        private IRepositoryEntitiesRepository _availableRepositories;
        private readonly AppProperties _appProperites;
        private ConcurrentDictionary<Guid, RepositoryEntity> _repositories =
            new ConcurrentDictionary<Guid, RepositoryEntity>();
        private ConcurrentDictionary<Guid, MavenSettings> _settings =
            new ConcurrentDictionary<Guid, MavenSettings>();


        public MavenServicesMapper(IRepositoryEntitiesRepository availableRepositories, AppProperties appProperites)
        {
            _availableRepositories = availableRepositories;
            _appProperites = appProperites;
            Refresh();
        }

        public void Refresh()
        {
            _repositories.Clear();
            _settings.Clear();

            foreach (var repo in _availableRepositories.GetByType("maven"))
            {
                var fullSettings = JsonConvert.DeserializeObject<MavenSettings>(repo.Settings);
                _settings[repo.Id] = fullSettings;
                _repositories[repo.Id] = repo;
            }
        }

        public int MaxRegistrationPages(Guid repoId)
        {
            return _settings[repoId].RegistrationPageSize;
        }

        public int MaxQueryPage(Guid repoId)
        {
            return _settings[repoId].QueryPageSize;
        }

        public int MaxCatalogPages(Guid repoId)
        {
            return _settings[repoId].CatalogPageSize;
        }

        public string ToMaven(Guid repoId, MavenIndex idx, bool search)
        {
            var repo = _repositories[repoId];
            var sett = _settings[repoId];

            if (search)
            {
                return sett.RemoteSearchAddress;
            }
            var result = sett.RemoteAddress.TrimEnd('/') + "/";
            if (idx.Group != null && idx.Group.Length > 0)
            {
                result += string.Join("/", idx.Group) + "/";
                if (!string.IsNullOrWhiteSpace(idx.Meta))
                {
                    if (!string.IsNullOrWhiteSpace(idx.ArtifactId))
                    {
                        result += idx.ArtifactId + "/";
                    }
                    result += idx.Meta;
                    if (!string.IsNullOrWhiteSpace(idx.Checksum))
                    {
                        result += "." + idx.Checksum;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(idx.ArtifactId))
                {
                    result += idx.ArtifactId + "/";
                    if (!string.IsNullOrWhiteSpace(idx.Version))
                    {
                        result += idx.Version;
                        if (idx.IsSnapshot)
                        {
                            result += "-SNAPSHOT";
                        }
                        result += "/";
                        if (!string.IsNullOrWhiteSpace(idx.Extension))
                        {
                            result += idx.ArtifactId;
                            
                            result += "-" + idx.Version;
                            if (idx.IsSnapshot)
                            {
                                result += "-SNAPSHOT";
                            }
                            if (!string.IsNullOrWhiteSpace(idx.Classifier))
                            {
                                result += "-" + idx.Classifier;
                            }
                            result += "." + idx.Extension;
                            if (!string.IsNullOrWhiteSpace(idx.Checksum))
                            {
                                result += "." + idx.Checksum;
                            }
                        }
                    }
                }
            }
            return result;
        }

        public bool HasTimestampedSnapshot(Guid repoId)
        {
            return !_settings[repoId].IsSingleSnapshot;
        }
    }
}
