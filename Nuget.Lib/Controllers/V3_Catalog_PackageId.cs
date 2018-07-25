
using MultiRepositories;
using MultiRepositories.Repositories;
using MultiRepositories.Service;
using Newtonsoft.Json;
using NugetProtocol;
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
        private ICatalogService _catalogService;
        private IServicesMapper _servicesMapper;
        private IRegistrationService _registrationService;
        private IRepositoryEntitiesRepository _reps;

        public V3_Catalog_PackageId(AppProperties properties,
            ICatalogService catalogService,
            IRepositoryEntitiesRepository reps,
            IRegistrationService registrationService,
            IServicesMapper servicesMapper, params string[] paths) :
            base(properties, null, paths)
        {
            _catalogService = catalogService;
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
            CatalogEntry result = null;
            //Registration340Entry
            if (repo.Mirror)
            {
                try
                {
                    result = GetPackageCatalogRemote(localRequest, repo);
                }
                catch (Exception)
                {

                }
            }
            if (result == null)
            {
                result = _catalogService.GetPackageCatalog(repo.Id,
                    localRequest.PathParams["date"], localRequest.PathParams["fullPackage"]);
            }
            return JsonResponse(result);
        }

        private CatalogEntry GetPackageCatalogRemote(SerializableRequest localRequest, RepositoryEntity repo)
        {
            CatalogEntry result;
            var remoteRequest = localRequest.Clone();
            var convertedUrl = _servicesMapper.ToNuget(repo.Id, localRequest.Protocol + "://" + localRequest.Host + localRequest.Url);
            remoteRequest.Headers["Host"] = new Uri(convertedUrl).Host;

            var path = localRequest.ToLocalPath("index.json");
            var remoteRes = RemoteRequest(convertedUrl, remoteRequest);

            result = JsonConvert.DeserializeObject<CatalogEntry>(Encoding.UTF8.GetString(remoteRes.Content));
            result.OId = _servicesMapper.FromNuget(repo.Id, result.OId);
            result.Id = _servicesMapper.FromNuget(repo.Id, result.Id);



            if (result.DependencyGroups != null)
            {
                foreach (var dg in result.DependencyGroups)
                {
                    dg.OId = _servicesMapper.FromNuget(repo.Id, dg.OId);
                    if (dg.Dependencies != null)
                    {
                        foreach (var dep in dg.Dependencies)
                        {
                            dep.Id = _servicesMapper.FromNuget(repo.Id, dep.Id);
                        }
                    }
                }
            }
            return result;
        }
    }
}
