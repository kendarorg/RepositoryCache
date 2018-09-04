using MultiRepositories.Repositories;
using MultiRepositories.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiRepositories;
using System.IO;
using Newtonsoft.Json;
using MavenProtocol.Apis;
using Maven.Services;
using MavenProtocol.Apis.Browse;
using MavenProtocol;
using HtmlAgilityPack;

namespace Maven.Controllers
{
    public class Maven2_Explore : ForwardRestApi
    {
        private readonly Guid repoId;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IRequestParser _requestParser;
        private readonly IMavenExploreService _mavenExploreService;
        private readonly IServicesMapper _servicesMapper;
        private readonly IMavenArtifactsService _mavenArtifactsService;

        public Maven2_Explore(Guid repoId, AppProperties properties,
            IRepositoryEntitiesRepository repositoryEntitiesRepository,
            IRequestParser requestParser, IMavenExploreService mavenExploreService, IServicesMapper servicesMapper,
            IMavenArtifactsService mavenArtifactsService, params string[] paths)
            : base(properties, null, paths)
        {
            this.repoId = repoId;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._requestParser = requestParser;
            this._mavenExploreService = mavenExploreService;
            this._servicesMapper = servicesMapper;
            this._mavenArtifactsService = mavenArtifactsService;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest arg)
        {

            var idx = _requestParser.Parse(arg);
            var repo = _repositoryEntitiesRepository.GetById(repoId);
            ExploreResult result = null;
            if (repo.Mirror)
            {
                try
                {
                    result = ExploreRemote(arg, repo, idx);
                }
                catch (Exception)
                {

                }
            }
            if (result == null)
            {
                result = _mavenExploreService.Explore(repo.Id, idx);
            }
            if (result.Content != null)
            {
                return new SerializableResponse
                {
                    Content = result.Content,
                    HttpCode = 200
                };
            }
            if ((arg.ContentType != null && arg.ContentType.Contains("text")) || (arg.Headers.ContainsKey("Accept") && arg.Headers["Accept"].Contains("text")))
            {
                return HtmlResponse(result);
            }
            return JsonResponse(result);
        }

        private ExploreResult ExploreRemote(SerializableRequest localRequest, RepositoryEntity repo, MavenIndex idx)
        {
            ExploreResult result = new ExploreResult();
            var remoteRequest = localRequest.Clone();
            var convertedUrl = _servicesMapper.ToMaven(repo.Id, idx, false);
            remoteRequest.Headers["Host"] = new Uri(convertedUrl).Host;

            var remoteRes = RemoteRequest(convertedUrl, remoteRequest);

            if (!string.IsNullOrWhiteSpace(idx.Meta))
            {
                result.Content = remoteRes.Content;
                var stringContent = Encoding.UTF8.GetString(remoteRes.Content);
                if (!string.IsNullOrWhiteSpace(idx.Checksum))
                {
                    _mavenArtifactsService.SetMetadataChecksums(repo.Id, idx, stringContent);
                }
                else
                {
                    _mavenArtifactsService.UpdateMetadata(repo.Id, idx, stringContent);
                }
            }
            else if (!string.IsNullOrWhiteSpace(idx.Type))
            {

                result.Content = remoteRes.Content;
                if (!string.IsNullOrWhiteSpace(idx.Checksum))
                {
                    var stringContent = Encoding.UTF8.GetString(remoteRes.Content);
                    _mavenArtifactsService.SetArtifactChecksums(repo.Id, idx, stringContent);
                }
                else
                {
                    _mavenArtifactsService.UploadArtifact(repo.Id, idx, remoteRes.Content);
                }
            }
            else
            {
                var htmlData = Encoding.UTF8.GetString(remoteRes.Content);
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlData);
                result.Children.Add("..");
                foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    result.Children.Add(link.Attributes["href"].Value);
                }
                if (idx.Group != null && idx.Group.Length > 0)
                {
                    result.Base = _properties.Host.TrimEnd('/') + "/" + repo.Prefix + "/" + string.Join("/", idx.Group);
                    if (!string.IsNullOrWhiteSpace(idx.ArtifactId))
                    {
                        result.Base += "/" + idx.ArtifactId;
                        if (!string.IsNullOrWhiteSpace(idx.Version))
                        {
                            var snap = idx.IsSnapshot ? "-SNAPSHOT" : "";
                            result.Base += "/" + idx.Version + snap;
                        }
                    }
                }
            }
            return result;
        }



        private SerializableResponse HtmlResponse(ExploreResult to)
        {
            var result = "<html>";
            result += "<head><base href='" + to.Base + "' ></head><body>";
            var spl = to.Base.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (spl.Length > 1)
            {
                result += "<a href='/"+string.Join("/",spl.Take(spl.Length-1))+"'>..</a><br>";
            }
            foreach (var item in to.Children)
            {
                result+= "<a href='"+ to.Base+"/"+item + "'>"+item+"</a><br>";
            }
            result += "</body></html>";
            return new SerializableResponse
            {
                Content = Encoding.UTF8.GetBytes(result),
                ContentType = "text/html",
                HttpCode = 200
            };
        }
    }
}
