
using MultiRepositories.Repositories;
using MultiRepositories.Service;
using Newtonsoft.Json;
//.Models;
//.Repositories;
//.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Controllers
{
    public class V3_Catalog_PackageId : ForwardRestApi
    {
        private AvailableRepositoriesRepository _reps;
        private UrlConverter _converter;
        private PackageRootRepository _packageRootRepository;
        private PackageDependencyRepository _packageDependencyRepository;

        public V3_Catalog_PackageId(
            PackageRootRepository packageRootRepository, PackageDependencyRepository packageDependencyRepository,
            AppProperties properties, UrlConverter converter, AvailableRepositoriesRepository reps) :
            base(properties, "/{repo}/v3/catalog/data/{date}/{fullPackage}.json", null)
        {
            _reps = reps;
            _converter = converter;
            _packageRootRepository = packageRootRepository;
            _packageDependencyRepository = packageDependencyRepository;
            SetHandler(Handle);
        }

        private SerializableResponse Handle(SerializableRequest arg)
        {
            //CatalogEntry
            var repo = _reps.GetByName(arg.PathParams["repo"]);
            var remote = arg.Clone();
            var convertedUrl = _converter.ToNuget(repo.Prefix, arg.Protocol + "://" + arg.Host + arg.Url);
            remote.Headers["Host"] = new Uri(convertedUrl).Host;

            SerializableResponse remoteRes = null;

            if (repo.Official && (!_properties.RunLocal ||arg.QueryParams.ContainsKey("runremote")))
            {
                try
                {
                    return OfficialResponse(arg, repo, remote, convertedUrl, out remoteRes);
                }
                catch (Exception)
                {
                    return RepoLocal(arg, repo);
                }
            }
            else
            {
                return RepoLocal(arg, repo);
            }
        }

        private SerializableResponse RepoLocal(SerializableRequest arg, AvailableRepositoryEntity repo)
        {
            var item = _packageRootRepository.GetByFullName(repo.Id, arg.PathParams["fullPackage"]);
            var lores = new CatalogEntry
            {
                Id = arg.Protocol + "://" + arg.Host + arg.Url,
                Context = new Context()
            };
            lores.Context.Vocab = _converter.LocalByType(repo.Prefix, "Schema/3.0.0");
            lores.Context.Catalog = _converter.LocalByType(repo.Prefix, "CatalogSchema/3.0.0");
            lores.Context.Xsd = _converter.LocalByType(repo.Prefix, "Xsd/3.0.0");

            lores.Context.Dependencies = new Dictionary<string, string> { { "@id", "dependency" }, { "@container", "@set" } };
            lores.Context.DependencyGroups = new Dictionary<string, string> { { "@id", "dependencyGroup" }, { "@container", "@set" } };
            lores.Context.PackageEntries = new Dictionary<string, string> { { "@id", "packageEntry" }, { "@container", "@set" } };
            lores.Context.SupportedFrameworks = new Dictionary<string, string> { { "@id", "supportedFramework" }, { "@container", "@set" } };
            lores.Context.Tags = new Dictionary<string, string> { { "@id", "catalog:tag" }, { "@container", "@set" } };
            lores.Context.Published = new CatalogEntry() { Type = "xsd:dateTime" };
            lores.Context.Created = new CatalogEntry() { Type = "xsd:dateTime" };
            lores.Context.LastEdited = new CatalogEntry() { Type = "xsd:dateTime" };
            lores.Context.CommitTimestamp = new CatalogEntry() { Type = "xsd:dateTime" };


            lores.Authors = item.Author;
            lores.CommitId = item.CommitId;
            lores.Id = item.PackageId;
            lores.CommitTimestamp = item.Timestamp;
            lores.Description = item.Description;
            lores.LicenseUrl = item.LicenseUrl;
            lores.MinClientVersion = item.MinClientVersion;
            lores.ProjectUrl = item.ProjectUrl;
            lores.Published = item.Published;
            lores.RequireLicenseAcceptance = item.RequireLicenseAcceptance;
            lores.Summary = item.Summary;
            if (!string.IsNullOrWhiteSpace(item.Tags))
            {
                lores.Tags = item.Tags.Split(',', ' ').ToList();
            }
            lores.Title = item.Title;
            lores.Version = item.Version;
            lores.IsPreRelease = item.IsPreRelease;
            lores.PackageHash = item.Hash;
            lores.PackageHashAlgorithm = item.HashAlgorithm;
            lores.PackageSize = item.PackageSize;
            lores.Listed = item.IsListed;
            lores.Language = "";

            foreach (var dep in _packageDependencyRepository.GetByPackageFullName(repo.Id, arg.PathParams["fullPackage"]))
            {
                //TODO
            }

            return JsonResponse(lores);
        }

        private SerializableResponse OfficialResponse(SerializableRequest arg, AvailableRepositoryEntity repo, SerializableRequest remote, string convertedUrl, out SerializableResponse remoteRes)
        {
            var path = arg.ToLocalPath();
            remoteRes = RemoteRequest(convertedUrl, remote);
            //Directory.CreateDirectory(Path.GetDirectoryName(path));
            //File.WriteAllBytes(path, remoteRes.Content);
            var result = JsonConvert.DeserializeObject<CatalogEntry>(Encoding.UTF8.GetString(remoteRes.Content));
            //result.Context.Base = _converter.FromNuget(result.Context.Base);
            result.Id = _converter.FromNuget(repo.Prefix, result.Id);
            result.Context = new Context
            {
                Vocab = _converter.LocalByType(repo.Prefix, "Schema/3.0.0"),
                Catalog = _converter.LocalByType(repo.Prefix, "CatalogSchema/3.0.0"),
                Xsd = _converter.LocalByType(repo.Prefix, "Xsd/3.0.0")
            };


            if (result.DependencyGroups != null)
            {
                foreach (var dg in result.DependencyGroups)
                {
                    dg.Id = _converter.FromNuget(repo.Prefix, dg.Id);
                    if (dg.Dependencies != null)
                    {
                        foreach (var dep in dg.Dependencies)
                        {
                            dep.Id = _converter.FromNuget(repo.Prefix, dep.Id);
                        }
                    }
                }
            }
            return JsonResponse(result);
        }
    }
}
