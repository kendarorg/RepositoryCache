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
    public class V3_Registration_Package_Version : ForwardRestApi
    {
        private AvailableRepositoriesRepository _reps;
        private UrlConverter _converter;
        private PackageRootRepository _packageRootRepository;

        public V3_Registration_Package_Version(
            PackageRootRepository queryRepository,
            AppProperties properties, UrlConverter converter, MultiRepositories.Repositories.AvailableRepositoriesRepository reps) : 
            base(properties, "/{repo}/v3/registration/{packageid}/{version}.json", null)
        {
            _reps = reps;
            _converter = converter;
            _packageRootRepository = queryRepository;
            SetHandler(Handle);
        }

        private SerializableResponse Handle(SerializableRequest arg)
        {
            var repo = _reps.GetByName(arg.PathParams["repo"]);
            //PackageEntry
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
            var pack = _packageRootRepository.GetByVersionId(repo.Id, arg.PathParams["packageid"], arg.PathParams["version"]);

            var date = pack.Published.ToString("yyyy.MM.dd.HH.mm.ss");


            var response = new PackageEntry()
            {
                Id = _converter.LocalByType(repo.Prefix, "RegistrationsBaseUrl") + "/" + arg.PathParams["packageid"] + "/" + arg.PathParams["version"] + ".json",
                CatalogEntry = _converter.LocalByType(repo.Prefix, "Catalog/3.0.0") + "/data/" + date + "/" + arg.PathParams["packageid"] + "." + arg.PathParams["version"] + ".json",
                PackageContent = _converter.LocalByType(repo.Prefix, "PackageBaseAddress/3.0.0") + "/" + arg.PathParams["packageid"] + "/" + arg.PathParams["version"] + "/" +
                        arg.PathParams["packageid"] + "." + arg.PathParams["version"] + ".nupkg",
                Registration = _converter.LocalByType(repo.Prefix, "RegistrationsBaseUrl") + "/" + arg.PathParams["packageid"] + "/index.json"
            };
            response.Context.Vocab = _converter.LocalByType(repo.Prefix, "Schema/3.0.0");
            response.Context.Xsd = _converter.LocalByType(repo.Prefix, "Xsd/3.0.0");
            return JsonResponse(response);
        }

        private SerializableResponse CreateRemote(SerializableRequest arg, AvailableRepositoryEntity repo, SerializableRequest remote, string convertedUrl)
        {
            SerializableResponse remoteRes = null;
            var path = arg.ToLocalPath();
            remoteRes = RemoteRequest(convertedUrl, remote);
            //Directory.CreateDirectory(Path.GetDirectoryName(path));
            //File.WriteAllBytes(path, remoteRes.Content);
            var result = JsonConvert.DeserializeObject<PackageEntry>(Encoding.UTF8.GetString(remoteRes.Content));
            result.Id = _converter.FromNuget(repo.Prefix, result.Id);
            result.CatalogEntry = _converter.FromNuget(repo.Prefix, result.CatalogEntry);
            result.PackageContent = _converter.FromNuget(repo.Prefix, result.PackageContent);
            result.Registration = _converter.FromNuget(repo.Prefix, result.Registration);

            return JsonResponse(result);
        }
    }
}
