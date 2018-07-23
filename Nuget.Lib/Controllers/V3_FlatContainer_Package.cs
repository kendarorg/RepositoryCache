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

namespace Nuget.Controllers
{
    public class V3_FlatContainer_Package : ForwardRestApi
    {
        private AvailableRepositoriesRepository _reps;
        private UrlConverter _converter;

        public V3_FlatContainer_Package(AppProperties properties, UrlConverter converter, MultiRepositories.Repositories.AvailableRepositoriesRepository reps) :
            base(properties, "/{repo}/v3/flatcontainer/{packageid}/index.json", null)
        {
            _reps = reps;
            _converter = converter;
            SetHandler(Handle);
        }

        private SerializableResponse Handle(SerializableRequest arg)
        {
            //FlatContainerVerions
            var repo = _reps.GetByName(arg.PathParams["repo"]);
            var remote = arg.Clone();
            var convertedUrl = _converter.ToNuget(repo.Prefix,arg.Protocol + "://" + arg.Host + arg.Url);
            
            if (repo.Official && (!_properties.RunLocal ||arg.QueryParams.ContainsKey("runremote")))
            {
                try
                {
                    return RemoteRequest(convertedUrl, remote);
                }
                catch (Exception)
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                //TODO
                throw new NotImplementedException();
            }
        }
    }
}
