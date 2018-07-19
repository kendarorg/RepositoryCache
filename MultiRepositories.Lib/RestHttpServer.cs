﻿using MultiRepositories;
using MultiRepositories.Commons;
using MultiRepositories.Repositories;

using MultiRepositories.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MultiRepositories
{
    public class RestHTTPServer : IRepositoryServiceProvider
    {
        private static int _counter = 0;
        private void Initialize()
        {

            var ap = new AppProperties();
            var arr = new AvailableRepositoriesRepository(ap);
            RegisterApi(new ServicesIndexApi(ap, arr));
            foreach (var item in _initializers)
            {
                item.Initialize(this, ap, arr);
            }
        }
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
        private IBaseServiceInitializer[] _initializers;

        public int Port
        {
            get { return _port; }
            private set { }
        }

        /// <summary>
        /// Construct server with given port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        /// <param name="port">Port of the server.</param>
        public RestHTTPServer(string path, int port, params IBaseServiceInitializer[] initializers)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            _initializers = initializers;


            this.Initialize(path, port);
        }

        /// <summary>
        /// Construct server with suitable port.
        /// </summary>
        /// <param name="path">Directory path to serve.</param>
        public RestHTTPServer(string path, params IBaseServiceInitializer[] initializers)
        {
            _initializers = initializers;
            //get an empty port
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            this.Initialize(path, port);
        }

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
            Initialize();
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:" + _port.ToString() + "/");
            _listener.Start();
            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static byte[] GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return new byte[] { };
            }
            using (System.IO.Stream body = request.InputStream) // here we have data
            {
                using (var ms = new MemoryStream())
                {
                    using (System.IO.BinaryReader reader = new System.IO.BinaryReader(body, request.ContentEncoding))
                    {
                        var offset = 0;
                        var read = reader.ReadBytes(10000);
                        while (read != null && read.Length > 0)
                        {
                            ms.Write(read, offset, read.Length);
                            offset += read.Length;
                        }
                        ms.Seek(0, SeekOrigin.Begin);
                        return ms.ToArray();
                    }
                }
            }
        }

        private void Process(HttpListenerContext context)
        {


            var sr = new SerializableRequest();
            var req = context.Request;

            var path = req.Url.AbsolutePath;
            if (path.ToLowerInvariant().EndsWith("favicon.ico"))
            {
                context.Response.StatusCode = 200;
                context.Response.OutputStream.Flush();
                context.Response.OutputStream.Close();
                return;
            }
            Console.WriteLine(req.Url);
            var filePath = path.TrimStart('/').Replace("/", "\\");
            //var host = path.TrimStart('/').Split('/').First();

            sr.Host = req.Url.Host + ":" + req.Url.Port;
            sr.Url = path;// "/" + string.Join("/", path.TrimStart('/').Split('/').Skip(1));

            //var nugetUrl = "https://" + host;




            HttpMethod method = HttpMethod.Get;

            sr.Method = req.HttpMethod.ToUpperInvariant();
            if (sr.Method == "GET") method = HttpMethod.Get;
            if (sr.Method == "POST") method = HttpMethod.Post;
            if (sr.Method == "PUT") method = HttpMethod.Put;
            //sr.ContentType = req.ContentType;
            sr.Headers = new Dictionary<String, String>();
            sr.QueryParams = new Dictionary<String, String>();

            foreach (string item in req.QueryString.Keys)
            {
                if (item == null)
                {
                    sr.QueryParams[item] = "true";
                }
                else
                {
                    sr.QueryParams[item] = req.QueryString[item];
                }
            }

            foreach (var reqh in req.Headers.AllKeys)
            {
                if (reqh == null) continue;
                sr.Headers.Add(reqh, req.Headers[reqh]);
            }
            sr.Content = GetRequestPostData(req);

            SerializableResponse res = null;
            _counter++;
            //File.WriteAllText(_counter + ".0.json", sr.Serialize());

            foreach (var api in _restApis)
            {
                if (api.CanHandleRequest(sr.Url))
                {
                    try
                    {
                        res = api.HandleRequest(sr);
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
                res = new SerializableResponse
                {
                    ContentType = "text",
                    Content = Encoding.UTF8.GetBytes("404 not found " + sr.Url),
                    HttpCode = 404
                };
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


        private void Initialize(string path, int port)
        {
            this._rootDirectory = path;
            this._port = port;
            _serverThread = new Thread(this.Listen);
            _serverThread.Start();
        }


    }
}
