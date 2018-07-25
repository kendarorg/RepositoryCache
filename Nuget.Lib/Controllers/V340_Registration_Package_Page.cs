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
    public class V340_Registration_Package_Page : ForwardRestApi
    {
        private IRepositoryEntitiesRepository _reps;
        private IRegistrationService _registrationPageRepository;
        private IServicesMapper _converter;

        public V340_Registration_Package_Page(
            AppProperties properties,
            IRegistrationService registrationPageRepository,
             IServicesMapper converter, IRepositoryEntitiesRepository reps,
            params string[] paths) :
            base(properties, null, paths)
        {
            _reps = reps;
            _registrationPageRepository = registrationPageRepository;
            _converter = converter;
            SetHandler(Handle);
        }

        private SerializableResponse Handle(SerializableRequest localRequest)
        {
            var semVerLevel = localRequest.QueryParams.ContainsKey("semVerLevel") ?
                 localRequest.QueryParams["semVerLevel"] : null;


            var repo = _reps.GetByName(localRequest.PathParams["repo"]);
            RegistrationPage result = null;
            //Registration340Entry
            if (repo.Mirror)
            {
                try
                {
                    result = SinglePageRemote(localRequest, repo);
                }
                catch (Exception)
                {

                }
            }
            if (result == null)
            {
                result = _registrationPageRepository.SinglePage(
                    repo.Id, localRequest.PathParams["packageid"]
                    , localRequest.PathParams["from"], localRequest.PathParams["to"], semVerLevel);
            }
            return JsonResponse(result);
        }

        private RegistrationPage SinglePageRemote(SerializableRequest localRequest, RepositoryEntity repo)
        {
            RegistrationPage result;
            var remoteRequest = localRequest.Clone();
            var convertedUrl = _converter.ToNuget(repo.Id, localRequest.Protocol + "://" + localRequest.Host + localRequest.Url);
            remoteRequest.Headers["Host"] = new Uri(convertedUrl).Host;

            var path = localRequest.ToLocalPath("index.json");
            var remoteRes = RemoteRequest(convertedUrl, remoteRequest);

            result = JsonConvert.DeserializeObject<RegistrationPage>(Encoding.UTF8.GetString(remoteRes.Content));
            //result.OContext.Base = _converter.FromNuget(repo.Id, result.Context.Base);
            result.OId = _converter.FromNuget(repo.Id, result.OId);
            foreach (var item in result.Items)
            {
                item.OId = _converter.FromNuget(repo.Id, item.OId);
                var ver = item;

                ver.OId = _converter.FromNuget(repo.Id, ver.OId);
                ver.CatalogEntry.OId = _converter.FromNuget(repo.Id, ver.CatalogEntry.OId);
                ver.CatalogEntry.PackageContent = _converter.FromNuget(repo.Id, ver.CatalogEntry.PackageContent);
                ver.Registration = _converter.FromNuget(repo.Id, ver.Registration);
                ver.PackageContent = _converter.FromNuget(repo.Id, ver.PackageContent);
                if (ver.CatalogEntry.DependencyGroups != null)
                {
                    foreach (var dg in ver.CatalogEntry.DependencyGroups)
                    {
                        dg.OId = _converter.FromNuget(repo.Id, dg.OId);
                        if (dg.Dependencies != null)
                        {
                            foreach (var de in dg.Dependencies)
                            {
                                de.Id = _converter.FromNuget(repo.Id, de.Id);
                            }
                        }
                    }
                }

            }
            return result;
        }
    }
}
