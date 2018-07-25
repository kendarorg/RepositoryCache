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
using NugetProtocol;
using MultiRepositories;

namespace Nuget.Controllers
{
    public class V3_Registration_Package_Version : ForwardRestApi
    {
        private IServicesMapper _servicesMapper;
        private IRegistrationService _registrationService;
        private IRepositoryEntitiesRepository _reps;

        public V3_Registration_Package_Version(AppProperties properties,
            IRepositoryEntitiesRepository reps,
            IRegistrationService registrationService,
            IServicesMapper servicesMapper, params string[] paths) :
            base(properties,  null,paths)
        {
            _servicesMapper = servicesMapper;
            _registrationService = registrationService;
            _reps = reps;
            SetHandler(Handle);
        }

        private SerializableResponse Handle(SerializableRequest localRequest)
        {
            var semVerLevel = localRequest.PathParams.ContainsKey("semver") ?
                 localRequest.PathParams["semver"] : null;


            var repo = _reps.GetByName(localRequest.PathParams["repo"]);
            RegistrationLastLeaf result = null;
            //Registration340Entry
            if (repo.Mirror)
            {
                try
                {
                    result = LeafRemote(localRequest, repo);
                }
                catch (Exception)
                {

                }
            }
            if (result == null)
            {
                var lowerIdVersion = localRequest.PathParams["packageid"]+"."+localRequest.PathParams["version"];
                result = _registrationService.Leaf(repo.Id, 
                    localRequest.PathParams["packageid"], localRequest.PathParams["version"], lowerIdVersion,
                    semVerLevel);
            }
            return JsonResponse(result);
        }

        private RegistrationLastLeaf LeafRemote(SerializableRequest localRequest, RepositoryEntity repo)
        {
            RegistrationLastLeaf result;
            var remoteRequest = localRequest.Clone();
            var convertedUrl = _servicesMapper.ToNuget(repo.Id, localRequest.Protocol + "://" + localRequest.Host + localRequest.Url);
            remoteRequest.Headers["Host"] = new Uri(convertedUrl).Host;

            var path = localRequest.ToLocalPath("index.json");
            var remoteRes = RemoteRequest(convertedUrl, remoteRequest);

            result = JsonConvert.DeserializeObject<RegistrationLastLeaf>(Encoding.UTF8.GetString(remoteRes.Content));
            result.OId = _servicesMapper.FromNuget(repo.Id, result.OId);
            result.CatalogEntry = _servicesMapper.FromNuget(repo.Id, result.CatalogEntry);
            result.PackageContent = _servicesMapper.FromNuget(repo.Id, result.PackageContent);
            result.Registration = _servicesMapper.FromNuget(repo.Id, result.Registration);
            return result;
        }
    }
}
