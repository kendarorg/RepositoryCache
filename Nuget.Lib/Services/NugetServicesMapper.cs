﻿using MultiRepositories.Repositories;
using Newtonsoft.Json;
using NugetProtocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Nuget
{
    public class NugetServicesMapper : IServicesMapper
    {
        private IRepositoryEntitiesRepository _availableRepositories;
        private ConcurrentDictionary<Guid, RepositoryEntity> _repositories =
            new ConcurrentDictionary<Guid, RepositoryEntity>();
        private ConcurrentDictionary<Guid, Dictionary<string, EntryPointDescriptor>> _entryPoints =
            new ConcurrentDictionary<Guid, Dictionary<string, EntryPointDescriptor>>();
        private ConcurrentDictionary<Guid, Dictionary<string, EntryPointDescriptor>> _shownApis =
            new ConcurrentDictionary<Guid, Dictionary<string, EntryPointDescriptor>>();

        public Dictionary<string, EntryPointDescriptor> GetVisibles(Guid id)
        {
            return _shownApis[id];
        }

        public NugetServicesMapper(IRepositoryEntitiesRepository availableRepositories)
        {
            _availableRepositories = availableRepositories;
            Refresh();
        }

        public void Refresh()
        {
            foreach (var repo in _availableRepositories.GetByType("nuget"))
            {
                _repositories[repo.Id] = repo;

                _entryPoints[repo.Id] = ExpandDescriptors(repo);
                _shownApis[repo.Id] = GetShown(_entryPoints[repo.Id]);
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
            var settings = JsonConvert.DeserializeObject<List<EntryPointDescriptor>>(repo.Settings);

            //Setup main items
            foreach (var descriptor in settings.Where(a => string.IsNullOrWhiteSpace(a.Ref)))
            {
                var copy = Clone(descriptor);
                result[copy.Id + ":" + copy.SemVer ?? ""] = copy;
            }

            //Setup ref items
            foreach (var descriptor in settings.Where(a => !string.IsNullOrWhiteSpace(a.Ref)))
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
    }
}