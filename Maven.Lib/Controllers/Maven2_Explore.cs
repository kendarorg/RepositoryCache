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
using Maven.News;
using MavenProtocol.News;
using System.Xml.Serialization;
using System.Xml;

namespace Maven.Controllers
{
    public class Maven2_Explore : ForwardRestApi
    {
        private readonly Guid _repoId;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IRequestParser _requestParser;
        private readonly IExploreApi _exploreApi;
        private readonly IPomApi _pomApi;
        private readonly IArtifactsApi _artifactsApi;
        private readonly IMetadataApi _metadataApi;
        private readonly IMetadataRepository _metadataRepository;
        private readonly IServicesMapper _servicesMapper;

        public Maven2_Explore(Guid repoId, AppProperties properties,
            IRepositoryEntitiesRepository repositoryEntitiesRepository, IServicesMapper servicesMapper,
            IRequestParser requestParser,
            IExploreApi exploreApi, IPomApi pomApi, IArtifactsApi artifactsApi, IMetadataApi metadataApi,
            IMetadataRepository metadataRepository,
            params string[] paths)
            : base(properties, null, paths)
        {
            this._repoId = repoId;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._requestParser = requestParser;
            this._exploreApi = exploreApi;
            this._pomApi = pomApi;
            this._artifactsApi = artifactsApi;
            this._metadataApi = metadataApi;
            this._metadataRepository = metadataRepository;
            this._servicesMapper = servicesMapper;
            SetHandler(Handler);
        }

        private SerializableResponse Handler(SerializableRequest arg)
        {
            try
            {
                var idx = _requestParser.Parse(arg);
                idx.RepoId = _repoId;
                var repo = _repositoryEntitiesRepository.GetById(_repoId);

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
                    if (idx.Group != null && idx.Group.Length > 1)
                    {
                        var supposedGroup = idx.Group.Take(idx.Group.Length - 1).Select(a=>a).ToArray();
                        var supposedArtifact = idx.Group.Last();
                        var meta = _metadataRepository.GetArtifactMetadata(repo.Id, supposedGroup, supposedArtifact);
                        if (meta != null)
                        {
                            idx.Version = idx.ArtifactId;
                            idx.Group = supposedGroup;
                            idx.ArtifactId = supposedArtifact;
                        }
                }
                    result = new ExploreResult
                    {
                        Base = "/" + repo.Prefix
                    };
                    if (idx.Group != null && idx.Group.Any())
                    {
                        result.Base += "/" + string.Join("/", idx.Group);
                        if (idx.ArtifactId != null)
                        {
                            result.Base += "/" + idx.ArtifactId;
                            if (idx.Version != null)
                            {
                                result.Base += "/" + idx.Version;
                                if (idx.IsSnapshot)
                                {
                                    result.Base += "-SNAPSHOT";
                                }
                            }
                        }
                    }
                    ExploreLocal(idx, result);
                    if( (result.Children==null ||result.Children.Count==0) && result.Content==null)
                    {
                        return new SerializableResponse
                        {
                            Content = Encoding.UTF8.GetBytes("Error page not found"),
                            ContentType = "text/plain",
                            HttpCode = 404
                        };
                    }
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
                if (result.Content == null && result.Children == null)
                {
                    return new SerializableResponse
                    {
                        Content = new byte[] { },
                        HttpCode = 200
                    };
                }
                return JsonResponse(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new NotFoundException();
            }
        }

        private void ExploreLocal(MavenIndex idx, ExploreResult result)
        {
            if (_artifactsApi.CanHandle(idx))
            {
                var re = _artifactsApi.Retrieve(idx);
                if (re != null)
                {
                    if (re.Content != null)
                    {
                        result.Content = re.Content;
                    }
                    else if (idx.Checksum == "md5")
                    {
                        result.Content = Encoding.UTF8.GetBytes(re.Md5);
                    }
                    else if (idx.Checksum == "sha1")
                    {
                        result.Content = Encoding.UTF8.GetBytes(re.Sha1);
                    }
                }
            }
            else if (_metadataApi.CanHandle(idx))
            {
                var re = _metadataApi.Retrieve(idx);
                if (re != null)
                {
                    if (re.Xml != null)
                    {
                        var xsSubmit = new XmlSerializer(re.Xml.GetType());

                        using (var sww = new StringWriter())
                        {
                            using (var writer = XmlWriter.Create(sww))
                            {
                                xsSubmit.Serialize(writer, re.Xml);
                                result.Content = Encoding.UTF8.GetBytes(sww.ToString());
                            }
                        }
                    }
                    else if (idx.Checksum == "md5")
                    {
                        result.Content = Encoding.UTF8.GetBytes(re.Md5);
                    }
                    else if (idx.Checksum == "sha1")
                    {
                        result.Content = Encoding.UTF8.GetBytes(re.Sha1);
                    }
                }
            }
            else if (_pomApi.CanHandle(idx))
            {
                var re = _pomApi.Retrieve(idx);
                if (re != null)
                {
                    if (re.Xml != null)
                    {
                        /*var xsSubmit = new XmlSerializer(re.Xml.GetType());

                        using (var sww = new StringWriter())
                        {
                            using (var writer = XmlWriter.Create(sww))
                            {
                                xsSubmit.Serialize(writer, re.Xml);
                                result.Content = Encoding.UTF8.GetBytes(sww.ToString());
                            }
                        }*/
                        result.Content = Encoding.UTF8.GetBytes(re.Xml);
                    }
                    else if (idx.Checksum == "md5")
                    {
                        result.Content = Encoding.UTF8.GetBytes(re.Md5);
                    }
                    else if (idx.Checksum == "sha1")
                    {
                        result.Content = Encoding.UTF8.GetBytes(re.Sha1);
                    }
                }
            }
            else
            {
                var re = _exploreApi.Retrieve(idx);

                result.Children = re.Children;
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

            if (_artifactsApi.CanHandle(idx))
            {
                _artifactsApi.Generate(idx, true);
            }
            else if (_metadataApi.CanHandle(idx))
            {
                _metadataApi.Generate(idx, true);
            }
            else if (_pomApi.CanHandle(idx))
            {
                _pomApi.Generate(idx, true);
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
            var spl = new string[] { };
            if (to.Base == null)
            {
                to.Base = string.Empty;
            }
            spl = to.Base.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (spl.Length > 1)
            {
                result += "<a href='/" + string.Join("/", spl.Take(spl.Length - 1)) + "'>..</a>\r\n";
                result += "\r\n";
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
