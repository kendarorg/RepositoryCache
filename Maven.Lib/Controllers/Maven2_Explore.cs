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
            try
            {
                var idx = _requestParser.Parse(arg);
                var repo = _repositoryEntitiesRepository.GetById(repoId);
                ExploreResult result = null;
                if (repo.Mirror)
                {
                    try
                    {
                        var baseStandard = "";
                        var baseurls = arg.Url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        if (baseurls.Length >= 1)
                        {
                            baseStandard = "/" + string.Join("/", baseurls.Take(baseurls.Length - 2));
                        }
                        result = ExploreRemote(arg, repo, idx, arg.Url);
                        result.Base = arg.Url;


                    }
                    catch (InconsistentRemoteDataException)
                    {
                        throw;
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
                    var path = string.Join("/", idx.Group);
                    if (!string.IsNullOrEmpty(idx.ArtifactId))
                    {
                        if (!string.IsNullOrWhiteSpace(path)) path += "/" + idx.ArtifactId;
                        if (!string.IsNullOrWhiteSpace(idx.Version))
                        {
                            path += "/" + idx.Version;
                            if (idx.IsSnapshot) path += "-SNAPSHOT";
                        }
                    }

                    return HtmlResponse(result, path, repo.Prefix + " " + (repo.Mirror ? "Mirror" : "Local"));
                }
                return JsonResponse(result);
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw new NotFoundException();
            }
        }

        private ExploreResult ExploreRemote(SerializableRequest localRequest, RepositoryEntity repo, MavenIndex idx, string baseStandard)
        {
            ExploreResult result = new ExploreResult
            {
                Base = baseStandard
            };
            var remoteRequest = localRequest.Clone();
            var convertedUrl = _servicesMapper.ToMaven(repo.Id, idx, false);
            remoteRequest.Headers["Host"] = new Uri(convertedUrl).Host;

            var remoteRes = RemoteRequest(convertedUrl, remoteRequest, 60000);

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

                result.Children = new List<string>();
                htmlDoc.LoadHtml(htmlData);

                foreach (HtmlNode link in htmlDoc.DocumentNode.SelectNodes("//a[@href]"))
                {
                    if (!link.Attributes["href"].Value.Contains(".."))
                    {
                        result.Children.Add(link.Attributes["href"].Value.Trim('/'));
                    }
                }
                /*if (idx.Group != null && idx.Group.Length > 0)
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
                }*/
            }
            return result;
        }



        private SerializableResponse HtmlResponse(ExploreResult to, string path, string repoName)
        {

            if (string.IsNullOrWhiteSpace(path))
            {
                path = "root";
            }
            var result = "<html>";
            result += "<head><base href='" + to.Base + "' >";
            result += "<title>" + repoName + ":" + path + "</title>";
            result += "<meta name='viewport' content='width = device - width, initial - scale = 1.0'>";
            result += "</head><body>";


            result += "<header><h1>" + path + "</h1></header><hr/><main><pre id='contents'>";
            var spl = to.Base.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (spl.Length > 1)
            {
                result += "<a href='/" + string.Join("/", spl.Take(spl.Length - 1)) + "'>..</a>\r\n";
            }
            foreach (var item in to.Children)
            {
                result += "<a href='" + to.Base.TrimEnd('/') + "/" + item + "'>" + item + "</a>\r\n";
            }
            result += "</pre></main><hr/></body></html>";
            return new SerializableResponse
            {
                Content = Encoding.UTF8.GetBytes(result),
                ContentType = "text/html",
                HttpCode = 200
            };
        }
    }
}
