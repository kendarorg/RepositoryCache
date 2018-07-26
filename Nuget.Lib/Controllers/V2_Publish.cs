using MultiRepositories.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;
using Nuget.Services;
using MultiRepositories.Repositories;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;

namespace Nuget.Controllers
{
    public class V2_Publish : RestAPI
    {
        private IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private IInsertNugetService _insertNugetService;

        public V2_Publish(IInsertNugetService insertNugetService, IRepositoryEntitiesRepository repositoryEntitiesRepository, params string[] paths)
            : base(null, paths)
        {
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            _insertNugetService = insertNugetService;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest localRequest)
        {
            var apiKey = localRequest.Headers["X-NuGet-ApiKey"];
            var repo = _repositoryEntitiesRepository.GetByName(localRequest.PathParams["repo"]);
            var streamContent = new StreamContent(new MemoryStream(localRequest.Content));
            streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(localRequest.Headers["content-type"]);

            var res = streamContent.ReadAsMultipartAsync();
            res.Wait();
            var provider = res.Result;
            var ms = new MemoryStream();

            foreach (var httpContent in provider.Contents)
            {
                var fileName = httpContent.Headers.ContentDisposition.FileName;
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    continue;
                }

                var sres = httpContent.ReadAsStreamAsync();
                sres.Wait();
                using (Stream fileContents = sres.Result)
                {
                    fileContents.CopyTo(ms);
                }
            }
            _insertNugetService.Insert(repo.Id, apiKey, ms.ToArray());
            return  new SerializableResponse
            {
                ContentType = "text/plain",
                HttpCode = 200,
                Content = new byte[] { }
            };

        }

    }
}
