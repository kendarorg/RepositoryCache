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
    public class V340_Registration_Package : ForwardRestApi
    {
        private AvailableRepositoriesRepository _reps;
        private PackageRootRepository _queryRepository;
        private RegistrationPageRepository _registrationPageRepository;
        private UrlConverter _converter;

        public V340_Registration_Package(
            PackageRootRepository queryRepository,
            RegistrationPageRepository registrationPageRepository,
            AppProperties properties, UrlConverter converter, MultiRepositories.Repositories.AvailableRepositoriesRepository reps) : 
            base(properties, "/{repo}/v340/registration/{packageid}/index.json", null)
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
            var convertedUrl = _converter.ToNuget(repo.Prefix,arg.Protocol + "://" + arg.Host + arg.Url);
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
        }

        private SerializableResponse CreateLocal(SerializableRequest arg, AvailableRepositoryEntity repo)
        {
            var entry = new Registration340Entry
            {
                Id = _converter.LocalByType(repo.Prefix, "RegistrationsBaseUrl/3.4.0") + "/" + arg.PathParams["packageid"] + "/index.json",
                Context = new Context()
            };
            entry.Context.Vocab = _converter.LocalByType(repo.Prefix, "Schema/3.0.0");
            entry.Context.Catalog = _converter.LocalByType(repo.Prefix, "CatalogSchema/3.0.0");
            entry.Context.Xsd = _converter.LocalByType(repo.Prefix, "Xsd/3.0.0");
            entry.Context.Tags = new Dictionary<string, string> { { "@id", "catalog:tag" }, { "@container", "@set" } };
            entry.Context.PackageTargetFrameworks = new Dictionary<string, string> { { "@id", "packageTargetFramework" }, { "@container", "@set" } };
            entry.Context.DependencyGroups = new Dictionary<string, string> { { "@id", "dependencyGroup" }, { "@container", "@set" } };
            entry.Context.Dependencies = new Dictionary<string, string> { { "@id", "dependency" }, { "@container", "@set" } };
            entry.Context.Items = new Dictionary<string, string> { { "@id", "catalog:item" }, { "@container", "@set" } };
            entry.Context.CommitTimeStamp = new Dictionary<string, string> { { "@id", "catalog:commitTimeStamp" }, { "@type", "xsd:dateTime" } };
            entry.Context.CommitId = new CommitId { Id = "catalog:commitId" };
            entry.Context.Count = new CommitId { Id = "catalog:count" };
            entry.Context.Parent = new Dictionary<string, string> { { "@id", "catalog:parent" }, { "@type", "@id" } };
            entry.Context.PackageContent = new CatalogEntry { Type = "@id" };
            entry.Context.Published = new CatalogEntry { Type = "xsd:dateTime" };
            entry.Context.Registration = new CatalogEntry { Type = "@id" };
            entry.Type = new string[] { "catalog:CatalogRoot", "PackageRegistration", "catalog:Permalink" };

            var all = _registrationPageRepository.GetAllByPackageId(repo.Id, arg.PathParams["packageid"]).ToList();
            entry.Count = all.Count;
            entry.CommitTimeStamp = all.Min(a => a.Timestamp);
            entry.CommitId = all.First(a => a.Timestamp == entry.CommitTimeStamp).CommitId;
            entry.Items = new List<Registration340EntryItem>();
            RegistrationPageEntity lastPage = null;
            foreach (var item in all)
            {
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
            }
            //throw new Exception("http://localhost:9080/nuget.org/v340/registration/alba.cscss/index.json ADD SUBITEMSSS!!!");
            if (all.Count == 1)
            {
                var first = entry.Items.First();
                first.Items = new List<Registration340SubItem>();
                foreach (var subITem in _queryRepository.GetByPackageIdVersions(repo.Id, arg.PathParams["packageid"], lastPage.StartVersion, lastPage.EndVersion))
                {
                    var vrr = new Registration340SubItem
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
                    };
                    var date = subITem.Published.ToString("yyyy.MM.dd.HH.mm.ss");
                    vrr.CatalogEntry = new CatalogEntry();
                    var lores = vrr.CatalogEntry;
                    lores.Type = "PackageDetails";
                    lores.Authors = subITem.Author;
                    lores.CommitId = subITem.CommitId;
                    lores.Id = _converter.LocalByType(repo.Prefix, "Catalog/3.0.0") + "/data/" + date + "/" + arg.PathParams["packageid"] + "." + subITem.Version + ".json";
                        //subITem.PackageId;
                    lores.CommitTimestamp = subITem.Timestamp;
                    lores.Description = subITem.Description;
                    lores.LicenseUrl = subITem.LicenseUrl;
                    lores.MinClientVersion = subITem.MinClientVersion;
                    lores.ProjectUrl = subITem.ProjectUrl;
                    lores.Published = subITem.Published;
                    lores.RequireLicenseAcceptance = subITem.RequireLicenseAcceptance;
                    lores.Summary = subITem.Summary;
                    if (!string.IsNullOrWhiteSpace(subITem.Tags))
                    {
                        lores.Tags = subITem.Tags.Split(',', ' ').ToList();
                    }
                    lores.Title = subITem.Title;
                    lores.Version = subITem.Version;
                    lores.IsPreRelease = subITem.IsPreRelease;
                    lores.PackageHash = subITem.Hash;
                    lores.PackageHashAlgorithm = subITem.HashAlgorithm;
                    lores.PackageSize = subITem.PackageSize;
                    lores.Listed = subITem.IsListed;
                    lores.Language = "";
                    first.Items.Add(vrr);
                }

            }
            return JsonResponse(entry);
        }

        private SerializableResponse CreateRemote(SerializableRequest arg, AvailableRepositoryEntity repo, SerializableRequest remote, string convertedUrl)
        {
            SerializableResponse remoteRes = null;
            var path = arg.ToLocalPath();
            remoteRes = RemoteRequest(convertedUrl, remote);
            // Directory.CreateDirectory(Path.GetDirectoryName(path));
            //File.WriteAllBytes(path, remoteRes.Content);
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
