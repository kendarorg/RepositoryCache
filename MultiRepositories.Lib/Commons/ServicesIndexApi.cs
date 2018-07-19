using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;
using MultiRepositories.Repositories;
using MultiRepositories.Models;
using MultiRepositories.Service;

namespace MultiRepositories.Commons
{
    public class ServicesIndexApi : RestAPI
    {
        private AppProperties _properties;
        private AvailableRepositoriesRepository _availableRepositoriesRepository;

        public ServicesIndexApi(AppProperties properties, AvailableRepositoriesRepository availableRepositoriesRepository) : base("/v1/index.json", null)
        {
            _properties = properties;
            _availableRepositoriesRepository = availableRepositoriesRepository;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest arg)
        {
            var result = new List<ServiceModel>();
            foreach (var item in _availableRepositoriesRepository.GetAll())
            {
                result.Add(new ServiceModel
                {
                    Id = item.Id.ToString(),
                    Official = item.Official,
                    Prefix = item.Prefix,
                    Type = item.Type.ToLowerInvariant(),
                    Address = _properties.Host + "/" + item.Prefix + "/" + item.Address.TrimStart('/')
                });
            }

            return JsonResponse(result);
        }
    }
}
