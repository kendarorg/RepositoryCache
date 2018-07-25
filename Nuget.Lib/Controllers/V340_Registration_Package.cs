﻿using Newtonsoft.Json;
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
using MultiRepositories;
using NugetProtocol;

namespace Nuget.Controllers
{
    public class V340_Registration_Package : ForwardRestApi
    {
        private IServicesMapper _servicesMapper;
        private IRegistrationService _registrationService;
        private IRepositoryEntitiesRepository _reps;

        public V340_Registration_Package(AppProperties properties,
            IRepositoryEntitiesRepository reps,
            IRegistrationService registrationService,
            IServicesMapper servicesMapper,params string[]paths) :
            base(properties,  null,paths)
        {
            _servicesMapper = servicesMapper;
            _registrationService = registrationService;
            _reps = reps;
            SetHandler(Handle);
        }

        private SerializableResponse Handle(SerializableRequest localRequest)
        {
            var semVerLevel = localRequest.QueryParams.ContainsKey("semVerLevel") ?
                 localRequest.QueryParams["semVerLevel"] : null;


            var repo = _reps.GetByName(localRequest.PathParams["repo"]);
            RegistrationIndex result = null;
            //Registration340Entry
            if (repo.Mirror)
            {
                try
                {
                    result = IndexPageRemote(localRequest, repo);
                }
                catch (Exception)
                {

                }
            }
            if (result == null)
            {
                result = _registrationService.IndexPage(repo.Id, localRequest.PathParams["packageid"], semVerLevel);
            }
            return JsonResponse(result);
        }

        private RegistrationIndex IndexPageRemote(SerializableRequest localRequest, RepositoryEntity repo)
        {
            RegistrationIndex result;
            var remoteRequest = localRequest.Clone();
            var convertedUrl = _servicesMapper.ToNuget(repo.Id, localRequest.Protocol + "://" + localRequest.Host + localRequest.Url);
            remoteRequest.Headers["Host"] = new Uri(convertedUrl).Host;

            var path = localRequest.ToLocalPath("index.json");
            var remoteRes = RemoteRequest(convertedUrl, remoteRequest);

            result = JsonConvert.DeserializeObject<RegistrationIndex>(Encoding.UTF8.GetString(remoteRes.Content));


            result.OId = _servicesMapper.FromNuget(repo.Id, result.OId);
            foreach (var item in result.Items)
            {
                item.OId = _servicesMapper.FromNuget(repo.Id, item.OId);
                foreach (var ver in item.Items)
                {
                    ver.OId = _servicesMapper.FromNuget(repo.Id, ver.OId);
                    ver.CatalogEntry.Id = _servicesMapper.FromNuget(repo.Id, ver.CatalogEntry.Id);
                    ver.CatalogEntry.PackageContent = _servicesMapper.FromNuget(repo.Id, ver.CatalogEntry.PackageContent);
                    ver.Registration = _servicesMapper.FromNuget(repo.Id, ver.Registration);
                    ver.PackageContent = _servicesMapper.FromNuget(repo.Id, ver.PackageContent);
                    if (ver.CatalogEntry.DependencyGroups != null)
                    {
                        foreach (var dg in ver.CatalogEntry.DependencyGroups)
                        {
                            dg.OId = _servicesMapper.FromNuget(repo.Id, dg.OId);
                            if (dg.Dependencies != null)
                            {
                                foreach (var de in dg.Dependencies)
                                {
                                    de.Id = _servicesMapper.FromNuget(repo.Id, de.Id);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
