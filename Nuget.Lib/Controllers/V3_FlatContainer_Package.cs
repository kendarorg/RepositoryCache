using Newtonsoft.Json;
//.Models;
//.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories.Repositories;
using MultiRepositories.Service;
using MultiRepositories;
using NugetProtocol;

namespace Nuget.Controllers
{
    public class V3_FlatContainer_Package : ForwardRestApi
    {
        private IPackageBaseAddressService _packageBaseAddressService;
        private IRepositoryEntitiesRepository _reps;
        private IServicesMapper _converter;
        
        public V3_FlatContainer_Package(AppProperties properties, 
            IServicesMapper converter, IRepositoryEntitiesRepository reps,
            IPackageBaseAddressService packageBaseAddressService,params string[]paths) :
            base(properties, null, paths)
        {
            _packageBaseAddressService = packageBaseAddressService;
            _reps = reps;
            _converter = converter;
            SetHandler(Handle);
        }

        private SerializableResponse Handle(SerializableRequest localRequest)
        {

            var semVerLevel = localRequest.QueryParams.ContainsKey("semVerLevel") ?
                 localRequest.QueryParams["semVerLevel"] : null;


            var repo = _reps.GetByName(localRequest.PathParams["repo"]);
            VersionsResult result = null;
            //Registration340Entry
            if (repo.Mirror)
            {
                try
                {
                    result = GetVersionsRemote(localRequest, repo);
                }
                catch (Exception)
                {

                }
            }
            if (result == null)
            {
                result = _packageBaseAddressService.GetVersions(repo.Id, localRequest.PathParams["packageid"]);
            }
            return JsonResponse(result);
        }

        private VersionsResult GetVersionsRemote(SerializableRequest localRequest, RepositoryEntity repo)
        {
            VersionsResult result;
            var remoteRequest = localRequest.Clone();
            var convertedUrl = _converter.ToNuget(repo.Id, localRequest.Protocol + "://" + localRequest.Host + localRequest.Url);
            remoteRequest.Headers["Host"] = new Uri(convertedUrl).Host;

            var remoteRes = RemoteRequest(convertedUrl, remoteRequest);

            result = JsonConvert.DeserializeObject<VersionsResult>(Encoding.UTF8.GetString(remoteRes.Content));
            return result;
        }
    }
}
