using MultiRepositories;
using MultiRepositories.Commons;
using MultiRepositories.Repositories;
using MultiRepositories.Service;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MultiRepositories
{
    public class SimpleHTTPServer : IRepositoryServiceProvider, ISimpleHTTPServer, IDisposable
    {
        private static int _counter = 0;

        private List<RestAPI> _restApis = new List<RestAPI>();
        private readonly string[] _indexFiles = {
            "index.html",
            "index.htm",
            "default.html",
            "default.htm",
            "index.md"
        };

        public void RegisterApi(RestAPI api)
        {
            _restApis.Add(api);
        }


        private Thread _serverThread;
        private string _rootDirectory;
        private HttpListener _listener;
        private int _port;
        //private IBaseServiceInitializer[] _initializers;

        public int Port
        {
            get { return _port; }
            private set { }
        }

        public SimpleHTTPServer(
            AppProperties appProperties,
            IEnumerable<IApiProvider> apiProviders
            )
        {
            _applicationPropertes = appProperties;
            _apiProviders = apiProviders.ToList();
        }

        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="port">Port of the server.</param>
        public void Start(
            string path, int port, bool logRequests, IEnumerable<string> urls, IEnumerable<string> ignores)
        {

            _path = path;
            _logRequests = logRequests;
            if (ignores == null || ignores.Count() == 0)
            {
                _ignoreUrl = new List<string>
                {
                    "schema.nuget.org",
                    "www.w3.org",
                };
            }
            else
            {
                _ignoreUrl = ignores.ToList();
            }
            if (urls == null || urls.Count() == 0)
            {
                _replaceableUrl = new List<string>
                {
                    "nuget.org",
                    "repo.maven.apache.org",
                    "npmjs.org"
                };
            }
            else
            {
                _replaceableUrl = urls.ToList();
            }
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;



            this.Initialize(path, port);
        }

        /// <summary>
        /// Construct server with suitable port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /*public SimpleHTTPServer(string path, params IBaseServiceInitializer[] initializers)
        {
            _initializers = initializers;
            //get an empty port
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            this.Initialize(path, port);
        }*/

        /// <summary>
        /// Stop server and dispose all functions.
        /// </summary>
        public void Stop()
        {
            _serverThread.Abort();
            _listener.Stop();
        }

        private void Listen()
        {
            foreach (var item in _apiProviders)
            {
                item.Initialize(this);
            }
            _listener = new HttpListener();
            _local = "http://localhost:" + _port.ToString();
            _listener.Prefixes.Add("http://localhost:" + _port.ToString() + "/");
            _listener.Start();
            
            while (true)
            {
                HttpListenerContext context = _listener.GetContext();
                Task.Run(() =>
                {
                    try
                    {
                        Process(context);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
            }
        }

        public static byte[] GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return new byte[] { };
            }
            using (var ms = new MemoryStream())
            {
                using (System.IO.BinaryReader reader = new System.IO.BinaryReader(request.InputStream, request.ContentEncoding))
                {
                    //var offset = 0;
                    var read = reader.ReadBytes(10000);
                    while (read != null && read.Length > 0)
                    {
                        ms.Write(read, 0, read.Length);
                        //offset += read.Length - 1;
                        read = reader.ReadBytes(10000);
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                    var reult = ms.ToArray();
                    return reult;
                }
            }
        }

        private void Process(HttpListenerContext context)
        {


            var serializedRequest = new SerializableRequest();
            var req = context.Request;

            var path = req.Url.AbsolutePath;
            if (path.ToLowerInvariant().EndsWith("favicon.ico"))
            {
                context.Response.StatusCode = 200;
                context.Response.OutputStream.Flush();
                context.Response.OutputStream.Close();
                return;
            }
            Console.WriteLine(req.HttpMethod + ":" + req.Url);
            var filePath = path.TrimStart('/').Replace("/", "\\");
            //var host = path.TrimStart('/').Split('/').First();

            serializedRequest.Host = req.Url.Host + ":" + req.Url.Port;
            serializedRequest.Url = path;// "/" + string.Join("/", path.TrimStart('/').Split('/').Skip(1));

            //var nugetUrl = "https://" + host;




            HttpMethod method = HttpMethod.Get;

            serializedRequest.Method = req.HttpMethod.ToUpperInvariant();
            if (serializedRequest.Method == "GET") method = HttpMethod.Get;
            if (serializedRequest.Method == "POST") method = HttpMethod.Post;
            if (serializedRequest.Method == "PUT") method = HttpMethod.Put;
            //sr.ContentType = req.ContentType;
            serializedRequest.Headers = new Dictionary<String, String>(StringComparer.InvariantCultureIgnoreCase);
            serializedRequest.QueryParams = new Dictionary<String, String>(StringComparer.InvariantCultureIgnoreCase);

            foreach (string item in req.QueryString.Keys)
            {
                if (item == null)
                {
                    serializedRequest.QueryParams[item] = "true";
                }
                else
                {
                    serializedRequest.QueryParams[item] = req.QueryString[item];
                }
            }

            foreach (var reqh in req.Headers.AllKeys)
            {
                if (reqh == null) continue;
                serializedRequest.Headers.Add(reqh, req.Headers[reqh]);
            }
            serializedRequest.Content = GetRequestPostData(req);

            SerializableResponse res = null;
            _counter++;

            foreach (var api in _restApis)
            {
                if (api.CanHandleRequest(serializedRequest.Url, serializedRequest.Method))
                {
                    try
                    {
                        res = api.HandleRequest(serializedRequest);
                    }
                    catch (Exception ex)
                    {
                        res = new SerializableResponse
                        {
                            ContentType = "text/plain",
                            HttpCode = 500,
                            Content = Encoding.UTF8.GetBytes(ex.ToString())
                        };
                    };
                    break;
                }
                //File.WriteAllText(_counter + ".1.json", res.Serialize());
            }

            if (res == null)
            {
                var lp = serializedRequest.ToLocalPath();
                if (lp.StartsWith("/http"))
                {
                    try
                    {
                        res = ReplicateRequest(serializedRequest, res);
                    }
                    catch (Exception)
                    {

                    }
                }

                if (res == null)
                {
                    res = new SerializableResponse
                    {
                        ContentType = "text",
                        Content = Encoding.UTF8.GetBytes("404 not found " + serializedRequest.Url),
                        HttpCode = 404
                    };
                }
            }
            context.Response.StatusCode = res.HttpCode;
            context.Response.ContentType = res.ContentType;
            foreach (var h in res.Headers)
            {
                context.Response.Headers.Add(h.Key, h.Value);
            }
            context.Response.ContentLength64 = res.Content.Length;
            context.Response.OutputStream.Write(res.Content, 0, res.Content.Length);
            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();


        }

        private SerializableResponse ReplicateRequest(SerializableRequest serializedRequest, SerializableResponse res)
        {
            var uri = new Uri("http://test" + serializedRequest.Url).Segments.Where(a => a != "/").Select(a => a.Trim('/')).ToArray();
            var protocol = uri[0];
            var server = uri[1];
            var realUri = protocol + "://" + server + "/" + string.Join("/", uri.Skip(2));
            var cloned = serializedRequest.Clone();
            cloned.Url = realUri;

            var localPath = Path.Combine(_path, serializedRequest.ToLocalPath());


            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(localPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                }

                if (_logRequests)
                {
                    var data = JsonConvert.SerializeObject(serializedRequest);
                    data = JValue.Parse(data).ToString(Formatting.Indented);
                    File.WriteAllText(localPath + ".request", data);
                }
                res = ForwardRequest(cloned);


                File.WriteAllBytes(localPath + ".response", res.Content);
                File.WriteAllText(localPath + ".mime", res.ContentType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                if (File.Exists(localPath + ".response"))
                {
                    res = new SerializableResponse
                    {
                        ContentType = File.ReadAllText(localPath + ".mime"),
                        Content = File.ReadAllBytes(localPath + ".response"),
                        HttpCode = 200
                    };
                }
            }

            return res;
        }

        private List<string> _textData = new List<string>
        {
            "text/javascript",
            "application/xml",
            "application/json",
            "application/javascript",
            "application/x-javascript",
            "text/html",
            "text/plain",
            "text/xml",
            "text/css"
        };

        private List<string> _replaceableUrl;

        private List<string> _ignoreUrl;
        private string _local;
        private readonly AppProperties _applicationPropertes;
        private readonly List<IApiProvider> _apiProviders;
        private string _path;
        private bool _logRequests;

        private SerializableResponse ForwardRequest(SerializableRequest serializableReuqest)
        {
            //throw new Exception();
            var serializableResponse = new SerializableResponse();
            var method = HttpMethod.Get;
            if (serializableReuqest.Method == "GET") method = HttpMethod.Get;
            if (serializableReuqest.Method == "POST") method = HttpMethod.Post;
            if (serializableReuqest.Method == "PUT") method = HttpMethod.Put;
            if (serializableReuqest.Method == "DELETE") method = HttpMethod.Delete;
            var qp = "";
            if (serializableReuqest.QueryParams.Any())
            {
                qp = "?" + string.Join("&", serializableReuqest.QueryParams.Select(kvp => kvp.Key + "=" + kvp.Value));
            }

            var requestMessage = new HttpRequestMessage(method, serializableReuqest.Url + qp);
            foreach (var requestHeader in serializableReuqest.Headers)
            {
                if (requestHeader.Key == "Host")
                {
                    var uri = new Uri(serializableReuqest.Url);
                    requestMessage.Headers.Add(requestHeader.Key, uri.Host);
                }
                else
                {
                    if (!requestHeader.Key.ToLowerInvariant().StartsWith("expire"))
                    {
                        requestMessage.Headers.Add(requestHeader.Key, requestHeader.Value);
                    }
                }
            }
            if (serializableReuqest.Method == "POST" || serializableReuqest.Method == "PUT")
            {
                requestMessage.Content = new ByteArrayContent(serializableReuqest.Content);
            }
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = new HttpClient(handler);
            var asyncResponse = client.SendAsync(requestMessage);
            asyncResponse.Wait();
            var realResponse = asyncResponse.Result;

            serializableResponse.ContentType = realResponse.Content.Headers.ContentType.MediaType;

            serializableResponse.Headers["Date"] = DateTime.Now.ToString("r");
            serializableResponse.Headers["Last-Modified"] = DateTime.Now.ToString("r");
            var realResponsedata = realResponse.Content.ReadAsByteArrayAsync();
            realResponsedata.Wait();
            serializableResponse.Content = realResponsedata.Result;

            if (_textData.Contains(serializableResponse.ContentType))
            {
                var txt = Encoding.UTF8.GetString(serializableResponse.Content);
                var target = Encoding.UTF8.GetString(serializableResponse.Content);
                var results = new List<Match>();
                //string txt = "this is my url http://www.google.com and visit this website and this is my url http://www.yahoo.com";
                foreach (Match item in Regex.Matches(txt, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?"))
                {
                    try
                    {
                        var uri = new Uri(item.Value);
                        if (_replaceableUrl.Any(u => uri.Host.ToLowerInvariant().Contains(u.ToLowerInvariant())))
                        {
                            if (!_ignoreUrl.Any(u => uri.Host.ToLowerInvariant().Contains(u.ToLowerInvariant())))
                            {
                                var newUri = uri.ToString().Replace(uri.Scheme + "://", _local + "/" + uri.Scheme + "/");
                                target = target.Replace(item.Value, newUri);
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                if (serializableResponse.ContentType == "application/json")
                {
                    target = JValue.Parse(target).ToString(Formatting.Indented);
                }
                serializableResponse.Content = Encoding.UTF8.GetBytes(target);
            }

            return serializableResponse;

        }

        private void Initialize(string path, int port)
        {
            this._rootDirectory = path;
            this._port = port;
            _serverThread = new Thread(this.Listen);
            _serverThread.Start();
        }

        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources, but leave the other methods
        // exactly as they are.
        ~SimpleHTTPServer()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free native resources if there are any.

        }
    }
}
