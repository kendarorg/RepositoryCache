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
using MultiRepositories.Repositories;
using MultiRepositories.Service;

namespace Nuget.Controllers
{
    public class V340_Registration_Package_Page : ForwardRestApi
    {
        private AvailableRepositoriesRepository _reps;
        private QueryRepository _queryRepository;
        private RegistrationPageRepository _registrationPageRepository;
        private UrlConverter _converter;

        public V340_Registration_Package_Page(
            QueryRepository queryRepository,
            RegistrationPageRepository registrationPageRepository,
            AppProperties properties, UrlConverter converter, MultiRepositories.Repositories.AvailableRepositoriesRepository reps) :
            base(properties, "/{repo}/v340/registration/{packageid}/page/{from}/{to}.json", null)
        {
            _reps = reps;
            _queryRepository = queryRepository;
            _registrationPageRepository = registrationPageRepository;
            _converter = converter;
            SetHandler(Handle);
        }

        private SerializableResponse Handle(SerializableRequest arg)
        {
            var repo = _reps.GetByName(arg.PathParams["repo"]);
            //Registration340Entry
            var remote = arg.Clone();
            var convertedUrl = _converter.ToNuget(repo.Prefix, arg.Protocol + "://" + arg.Host + arg.Url);
            remote.Headers["Host"] = new Uri(convertedUrl).Host;

            if (repo.Official && (!_properties.RunLocal ||arg.QueryParams.ContainsKey("runremote")))
            {
                try
                {
                    return CreateRemote(arg, repo, remote, convertedUrl);
                }
                catch (Exception)
                {
                    return CreateLocal(arg, repo);
                }
            }
            else
            {
                return CreateLocal(arg, repo);
            }
            /*if (File.Exists(path))
            {
                remoteRes = new SerializableResponse()
                {
                    Content = File.ReadAllBytes(path),
                    HttpCode = 200
                };
            }
            else
            {
                throw ex;
            }*/


        }

        private SerializableResponse CreateLocal(SerializableRequest arg, AvailableRepositoryEntity repo)
        {
            var entry = new Registration340Entry();
            entry.Context = new Context
            {
                Vocab = _converter.LocalByType(repo.Prefix, "Schema/3.0.0"),
                Catalog = _converter.LocalByType(repo.Prefix, "CatalogSchema/3.0.0"),
                Xsd = _converter.LocalByType(repo.Prefix, "Xsd/3.0.0"),
                Tags = new Dictionary<string, string> { { "@id", "catalog:tag" }, { "@container", "@set" } },
                PackageTargetFrameworks = new Dictionary<string, string> { { "@id", "packageTargetFramework" }, { "@container", "@set" } },
                DependencyGroups = new Dictionary<string, string> { { "@id", "dependencyGroup" }, { "@container", "@set" } },
                Dependencies = new Dictionary<string, string> { { "@id", "dependency" }, { "@container", "@set" } },
                Items = new Dictionary<string, string> { { "@id", "catalog:item" }, { "@container", "@set" } },
                CommitTimeStamp = new Dictionary<string, string> { { "@id", "catalog:commitTimeStamp" }, { "@type", "xsd:dateTime" } },
                CommitId = new CommitId { Id = "catalog:commitId" },
                Count = new CommitId { Id = "catalog:count" },
                Parent = new Dictionary<string, string> { { "@id", "catalog:parent" }, { "@type", "@id" } },
                PackageContent = new CatalogEntry { Type = "@id" },
                Published = new CatalogEntry { Type = "xsd:dateTime" },
                Registration = new CatalogEntry { Type = "@id" }
            };
            entry.Type = new string[] { "catalog:CatalogRoot", "PackageRegistration", "catalog:Permalink" };

            var item = _registrationPageRepository.GetByPackageIdVersion(repo.Id,
                arg.PathParams["packageid"],
                arg.PathParams["from"],
                arg.PathParams["to"]);
            entry.Count = 1;
            entry.CommitTimeStamp = item.Timestamp;
            entry.CommitId = item.CommitId;
            entry.Items = new List<Registration340EntryItem>();
            RegistrationPageEntity lastPage = null;
            lastPage = item;
            entry.Items.Add(new Registration340EntryItem
            {
                CommitId = item.CommitId,
                CommitTimeStamp = item.Timestamp,
                Count = item.Count,
                Lower = item.StartVersion,
                Upper = item.EndVersion,
                Parent = arg.Protocol + "://" + arg.Host + arg.Url,
                Type = "catalog:CatalogPage",
                Id = _converter.LocalByType(repo.Prefix, "RegistrationsBaseUrl/3.4.0") + "/" + arg.PathParams["packageid"] +
                        "/page/" + item.StartVersion + "/" + item.EndVersion + ".json"
            });

            var first = entry.Items.First();
            first.Items = new List<Registration340SubItem>();
            foreach (var subITem in _queryRepository.GetByPackageIdVersions(repo.Id, arg.PathParams["packageid"], item.StartVersion, item.EndVersion))
            {
                first.Items.Add(new Registration340SubItem
                {
                    Id = _converter.LocalByType(repo.Prefix, "RegistrationsBaseUrl/3.4.0") +
                        "/" + arg.PathParams["packageid"] + "/" + subITem.Version + ".json",
                    Type = "Package",
                    CommitId = Guid.NewGuid().ToString(),
                    CommitTimeStamp = DateTime.UtcNow,
                    PackageContent = _converter.LocalByType(repo.Prefix, "PackageBaseAddress/3.0.0") +
                        "/" + arg.PathParams["packageid"] + "/" + subITem.Version + "/" + arg.PathParams["packageid"] + "." + subITem.Version + ".nupkg",
                    Registration = _converter.LocalByType(repo.Prefix, "RegistrationsBaseUrl/3.4.0") +
                        "/" + arg.PathParams["packageid"] + "/index.json",
                });
            }
            return JsonResponse(entry);
        }

        private SerializableResponse CreateRemote(SerializableRequest arg, AvailableRepositoryEntity repo, SerializableRequest remote, string convertedUrl)
        {
            SerializableResponse remoteRes = null;
            var path = arg.ToLocalPath();
            remoteRes = RemoteRequest(convertedUrl, remote);
            // Directory.CreateDirectory(Path.GetDirectoryName(path));
            // File.WriteAllBytes(path, remoteRes.Content);
            var result = JsonConvert.DeserializeObject<Registration340Entry>(Encoding.UTF8.GetString(remoteRes.Content));
            result.Context.Base = _converter.FromNuget(repo.Prefix, result.Context.Base);
            result.Id = _converter.FromNuget(repo.Prefix, result.Id);
            foreach (var item in result.Items)
            {
                item.Id = _converter.FromNuget(repo.Prefix, item.Id);
                foreach (var ver in item.Items)
                {
                    ver.Id = _converter.FromNuget(repo.Prefix, ver.Id);
                    ver.CatalogEntry.CatalogEntryId = _converter.FromNuget(repo.Prefix, ver.CatalogEntry.CatalogEntryId);
                    ver.CatalogEntry.PackageContent = _converter.FromNuget(repo.Prefix, ver.CatalogEntry.PackageContent);
                    ver.Registration = _converter.FromNuget(repo.Prefix, ver.Registration);
                    ver.PackageContent = _converter.FromNuget(repo.Prefix, ver.PackageContent);
                    if (ver.CatalogEntry.DependencyGroups != null)
                    {
                        foreach (var dg in ver.CatalogEntry.DependencyGroups)
                        {
                            dg.Id = _converter.FromNuget(repo.Prefix, dg.Id);
                            if (dg.Dependencies != null)
                            {
                                foreach (var de in dg.Dependencies)
                                {
                                    de.Id = _converter.FromNuget(repo.Prefix, de.Id);
                                    de.Registration = _converter.FromNuget(repo.Prefix, de.Registration);
                                }
                            }
                        }
                    }
                }
            }

            return JsonResponse(result);
        }
    }
}
