using Ioc;
using MultiRepositories;
using MultiRepositories.Repositories;
using NugetProtocol;
using System;
using System.Collections.Generic;

namespace Nuget
{
    public class NugetServiceIndex : IIndexService, ISingleton
    {
        private IRepositoryEntitiesRepository _repositoryEntities;
        private AppProperties _properties;
        private IServicesMapper _servicesMapper = null;

        public NugetServiceIndex(
            IRepositoryEntitiesRepository repositoryEntities,
            IServicesMapper servicesMapper,AppProperties properties)
        {
            _repositoryEntities = repositoryEntities;
            _properties = properties;
            _servicesMapper = servicesMapper;
        }

        public ServiceIndex Get(Guid repoId)
        {
            var repo = _repositoryEntities.GetById(repoId);
            var visibles =_servicesMapper.GetVisibles(repoId);
            var services = new List<Service>();
            foreach(var item in visibles.Values)
            {
                var local = item.Local.TrimStart('/').Replace("{repoName}",repo.Prefix);
                services.Add(new Service(local, item.Id, item.Comment));
            }
            return new ServiceIndex("3.0.0", services);
        }
    }
}
