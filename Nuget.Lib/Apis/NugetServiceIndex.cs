using MultiRepositories;
using NugetProtocol;
using System;
using System.Collections.Generic;

namespace Nuget
{
    public class NugetServiceIndex : IIndexService
    {
        private AppProperties _properties;
        private IServicesMapper _servicesMapper = null;

        public NugetServiceIndex(IServicesMapper servicesMapper,AppProperties properties)
        {
            _properties = properties;
            _servicesMapper = servicesMapper;
        }

        public ServiceIndex Get(Guid repoId)
        {
            var visibles =_servicesMapper.GetVisibles(repoId);
            var services = new List<Service>();
            foreach(var item in visibles.Values)
            {
                var local = _properties.Host+"/"+ item.Local.TrimStart('/');
                services.Add(new Service(local, item.Id, item.Comment));
            }
            return new ServiceIndex("3.0.0", services);
        }
    }
}
