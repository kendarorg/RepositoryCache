using Ionic.Zip;
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
using Nuget.Services;

namespace Nuget.Controllers
{
    public class V3_FlatContainer_Package_Version_Nupkg : ForwardRestApi
    {
        private readonly Guid repoId;
        private readonly IInsertNugetService _insertNugetService;
        private readonly Nuget.Repositories.IPackagesRepository _packagesRepository;
        private readonly IPackageBaseAddressService _nugetService;
        private readonly IServicesMapper _converter;
        private readonly IRepositoryEntitiesRepository _reps;

        public V3_FlatContainer_Package_Version_Nupkg(Guid repoId,
            AppProperties properties,
            IInsertNugetService insertNugetService,
            Nuget.Repositories.IPackagesRepository packagesRepository,
            IPackageBaseAddressService nugetService,
             IServicesMapper converter, IRepositoryEntitiesRepository reps,params string[]paths) :
            base(properties, null,paths)
        {
            this.repoId = repoId;
            _insertNugetService = insertNugetService;
            _packagesRepository = packagesRepository;
            this._nugetService = nugetService;
            this._converter = converter;
            _reps = reps;
            SetHandler(Handle);
        }

        private SerializableResponse Handle(SerializableRequest localRequest)
        {
            var semVerLevel = localRequest.QueryParams.ContainsKey("semVerLevel") ?
                 localRequest.QueryParams["semVerLevel"] : null;


            var repo = _reps.GetById(repoId);
            byte[] result = null;
            //Registration340Entry
            if (repo.Mirror && _properties.IsOnline(localRequest))
            {
                try
                {
                    result = GetNupkgRemote(localRequest, repo);
                    var previous = _packagesRepository.GetByIdVersion(repo.Id, localRequest.PathParams["fullversion"]);
                    if (previous == null)
                    {
                        _insertNugetService.Insert(repo.Id, null, result);
                    }
                    else if (previous.Size != result.Length)
                    {
                        try
                        {
                            _insertNugetService.Insert(repo.Id, null, result);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            if (result == null)
            {
                result = _nugetService.GetNupkg(repo.Id, localRequest.PathParams["fullversion"]);
            }
            return new SerializableResponse()
            {
                Content = result,
                HttpCode = 200
            };
        }

        private byte[] GetNupkgRemote(SerializableRequest localRequest, RepositoryEntity repo)
        {
            var remoteRequest = localRequest.Clone();
            var convertedUrl = _converter.ToNuget(repo.Id, localRequest.Protocol + "://" + localRequest.Host + localRequest.Url);
            remoteRequest.Headers["Host"] = new Uri(convertedUrl).Host;

            var remoteRes = RemoteRequest(convertedUrl, remoteRequest);
            return remoteRes.Content;
        }
    }
}
