using Ioc;
using MultiRepositories;
using MultiRepositories.Repositories;
using Newtonsoft.Json;
using NugetProtocol;
using NugetProtocol.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Nuget
{
    public class NugetServicesMapper : IServicesMapper, ISingleton
    {
        private IRepositoryEntitiesRepository _availableRepositories;
        private AppProperties _appProperites;
        private ConcurrentDictionary<Guid, RepositoryEntity> _repositories =
            new ConcurrentDictionary<Guid, RepositoryEntity>();
        private ConcurrentDictionary<Guid, Dictionary<string, EntryPointDescriptor>> _entryPoints =
            new ConcurrentDictionary<Guid, Dictionary<string, EntryPointDescriptor>>();
        private ConcurrentDictionary<Guid, Dictionary<string, EntryPointDescriptor>> _shownApis =
            new ConcurrentDictionary<Guid, Dictionary<string, EntryPointDescriptor>>();
        private ConcurrentDictionary<Guid, List<EntryPointDescriptor>> _toNugetApi =
            new ConcurrentDictionary<Guid, List<EntryPointDescriptor>>();
        private ConcurrentDictionary<Guid, List<EntryPointDescriptor>> _fromNugetApi =
            new ConcurrentDictionary<Guid, List<EntryPointDescriptor>>();
        private ConcurrentDictionary<Guid, NugetSettings> _settings =
            new ConcurrentDictionary<Guid, NugetSettings>();

        public Dictionary<string, EntryPointDescriptor> GetVisibles(Guid id)
        {
            return _shownApis[id];
        }

        public NugetServicesMapper(IRepositoryEntitiesRepository availableRepositories, AppProperties appProperites)
        {
            _availableRepositories = availableRepositories;
            _appProperites = appProperites;
            Refresh();
        }

        public void Refresh()
        {
            _repositories.Clear();
            _entryPoints.Clear();
            _shownApis.Clear();
            _toNugetApi.Clear();
            _fromNugetApi.Clear();
            _settings.Clear();

            foreach (var repo in _availableRepositories.GetByType("nuget"))
            {
                _repositories[repo.Id] = repo;

                _entryPoints[repo.Id] = ExpandDescriptors(repo);
                _shownApis[repo.Id] = GetShown(_entryPoints[repo.Id]);
                _toNugetApi[repo.Id] = new List<EntryPointDescriptor>();
                foreach (var item in _shownApis[repo.Id].Values.OrderByDescending(a => a.Local))
                {
                    _toNugetApi[repo.Id].Add(item);
                }
                _fromNugetApi[repo.Id] = new List<EntryPointDescriptor>();
                foreach (var item in _shownApis[repo.Id].Values.OrderByDescending(a => a.Remote))
                {
                    _fromNugetApi[repo.Id].Add(item);
                }
            }
        }

        public string From(Guid repoId, string resourceId, params string[] pars)
        {
            var repo = _repositories[repoId];
            var result = string.Empty;
            switch (resourceId)
            {
                case ("*SchemaTime"):
                    result = "http://www.w3.org/2001/XMLSchema#dateTime";
                    break;
                case ("*CatalogPermalink"):
                    result = "http://schema.nuget.org/catalog#Permalink";
                    break;
                case ("*Catalog"):
                    result = "http://schema.nuget.org/catalog#";
                    break;
                case ("*Schema"):
                    result = "http://schema.nuget.org/schema#";
                    break;
                case ("*W3SchemaComment"):
                    result = "http://www.w3.org/2000/01/rdf-schema#comment";
                    break;
                default:
                    if (!_entryPoints[repoId].ContainsKey(resourceId))
                    {
                        return null;
                    }
                    result = _entryPoints[repoId][resourceId].Local.
                        Replace("{repoName}", _repositories[repo.Id].Prefix);
                    break;
            }

            if (pars.Length > 0)
            {
                return result + "/" + string.Join("/", pars);
            }
            return result;
        }

        public string FromSemver(Guid repoId, string resourceId, string semVerLevel, params string[] par)
        {
            if (string.IsNullOrWhiteSpace(semVerLevel))
            {
                semVerLevel = string.Empty;
            }
            var guess = From(repoId, resourceId + ":" + semVerLevel, par);
            if (string.IsNullOrWhiteSpace(guess))
            {
                guess = From(repoId, resourceId, par);
            }
            return guess;
        }


        private Dictionary<string, EntryPointDescriptor> ExpandDescriptors(RepositoryEntity repo)
        {
            var result = new Dictionary<string, EntryPointDescriptor>(StringComparer.InvariantCultureIgnoreCase);
            var fullSettings = JsonConvert.DeserializeObject<NugetSettings>(repo.Settings);
            _settings[repo.Id] = fullSettings;
            var services = fullSettings.Services;
            foreach (var descriptor in services)
            {
                if (!string.IsNullOrWhiteSpace(descriptor.Local))
                {
                    descriptor.Local = _appProperites.Host.TrimEnd('/') + "/" + descriptor.Local.TrimStart('/');
                }
            }
            //Setup main items
            foreach (var descriptor in services.Where(a => string.IsNullOrWhiteSpace(a.Ref)))
            {
                var copy = Clone(descriptor);
                result[copy.Id + ":" + copy.SemVer ?? ""] = copy;
            }

            //Setup ref items
            foreach (var descriptor in services.Where(a => !string.IsNullOrWhiteSpace(a.Ref)))
            {
                var copy = Clone(descriptor);
                var referenceId = "";
                EntryPointDescriptor reference = Clone(descriptor);
                while (!string.IsNullOrWhiteSpace(reference.Ref))
                {
                    referenceId = reference.Ref + ":" + reference.SemVer;
                    reference = result[referenceId];
                    copy.Local = copy.Local ?? reference.Local;
                    copy.Remote = copy.Remote ?? reference.Remote;
                    copy.Comment = string.IsNullOrWhiteSpace(copy.Comment) ? reference.Comment : copy.Comment;
                    copy.RemoteAlternative = copy.RemoteAlternative ?? reference.RemoteAlternative;
                    copy.SemVer = copy.SemVer ?? reference.SemVer;
                }

                result[copy.Id + ":" + copy.SemVer ?? ""] = copy;
            }
            foreach (var item in result.ToArray())
            {
                result[item.Value.Id] = item.Value;
            }
            return result;
        }

        private static EntryPointDescriptor Clone(EntryPointDescriptor descriptor)
        {
            return new EntryPointDescriptor
            {
                Compress = descriptor.Compress,
                Id = descriptor.Id,
                Visible = descriptor.Visible,
                Local = descriptor.Local,
                Ref = descriptor.Ref,
                Comment = descriptor.Comment,
                Remote = descriptor.Remote,
                RemoteAlternative = descriptor.RemoteAlternative,
                SemVer = string.IsNullOrWhiteSpace(descriptor.SemVer) ? null : descriptor.SemVer
            };
        }

        private Dictionary<string, EntryPointDescriptor> GetShown(Dictionary<string, EntryPointDescriptor> dictionary)
        {
            var result = new Dictionary<string, EntryPointDescriptor>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var item in dictionary)
            {
                if (item.Value.Visible && item.Key.IndexOf(":") < 0)
                {
                    result[item.Value.Id] = item.Value;
                }
            }
            return result;
        }

        public string ToNuget(Guid repoId, String src)
        {
            if (src == null) return null;
            var srcCompare = src.TrimEnd('/') + "/";
            var repo = _repositories[repoId];

            foreach (var item in _toNugetApi[repoId])
            {
                var remoCmp = item.Remote.TrimEnd('/') + "/";
                var localCmp = item.Local.Replace("{repoName}", repo.Prefix).TrimEnd('/') + "/";

                if (srcCompare.StartsWith(localCmp))
                {
                    return src.Replace(item.Local.Replace("{repoName}", repo.Prefix), item.Remote);
                }
            }
            return src;
        }

        public string FromNuget(Guid repoId, string src)
        {
            if (src == null) return null;
            var srcCompare = src.TrimEnd('/') + "/";
            var repo = _repositories[repoId];
            foreach (var item in _fromNugetApi[repoId])
            {
                var remoCmp = item.Remote.TrimEnd('/') + "/";
                var remoAltCmp = (item.RemoteAlternative ?? "").TrimEnd('/') + "/";
                var localCmp = item.Local.Replace("{repoName}", repo.Prefix).TrimEnd('/') + "/";


                if (srcCompare.StartsWith(remoCmp))
                {
                    return src.Replace(item.Remote, item.Local.Replace("{repoName}", repo.Prefix));
                }
                else if (!string.IsNullOrWhiteSpace(remoAltCmp) && srcCompare.StartsWith(remoAltCmp))
                {
                    return src.Replace(item.RemoteAlternative, item.Local.Replace("{repoName}", repo.Prefix));
                }
            }
            return src;
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
    }
}
