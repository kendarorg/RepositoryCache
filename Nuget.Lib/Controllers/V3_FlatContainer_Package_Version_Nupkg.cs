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

namespace Nuget.Controllers
{
    public class V3_FlatContainer_Package_Version_Nupkg : ForwardRestApi
    {

        private readonly NugetService _nugetService;
        private readonly UrlConverter _converter;
        private readonly AvailableRepositoriesRepository _reps;

        public V3_FlatContainer_Package_Version_Nupkg(
            NugetService nugetService,
            AppProperties properties, UrlConverter converter, MultiRepositories.Repositories.AvailableRepositoriesRepository reps) :
            base(properties, "/{repo}/v3/flatcontainer/{id-lower}/{version-lower}/{fullversion}.nupkg", null)
        {
            this._nugetService = nugetService;
            this._converter = converter;
            _reps = reps;
            SetHandler(Handle);
        }

        private SerializableResponse Handle(SerializableRequest arg)
        {
            //Nupackage
            var repo = _reps.GetByName(arg.PathParams["repo"]);
            var packageId = arg.PathParams["id-lower"];
            var fullversion = arg.PathParams["fullversion"];
            var remote = arg.Clone();
            var convertedUrl = _converter.ToNuget(repo.Prefix, arg.Protocol + "://" + arg.Host + arg.Url);
            SerializableResponse remoteRes = null;
            var path = arg.ToLocalPath();

            var fileName = Path.Combine(_properties.NupkgDir, repo.Id.ToString(), packageId, fullversion+".nupkg");

            if (repo.Official && (!_properties.RunLocal ||arg.QueryParams.ContainsKey("runremote")))
            {
                try
                {
                    remoteRes = RemoteRequest(convertedUrl, remote);
                    _nugetService.InsertNuget(remoteRes.Content, repo);
                }
                catch (Exception)
                {
                    remoteRes = new SerializableResponse()
                    {
                        Content = File.ReadAllBytes(fileName),
                        HttpCode = 200
                    };
                }
            }
            else
            {
                remoteRes = new SerializableResponse()
                {
                    Content = File.ReadAllBytes(fileName),
                    HttpCode = 200
                };
            }
            return remoteRes;
        }

    }
}
