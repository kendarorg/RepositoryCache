
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace MultiRepositories.Service
{
    public abstract class ForwardRestApi : RestAPI
    {
        protected AppProperties _properties;


        public Func<String, SerializableRequest, SerializableResponse> RequestData { get; set; }

        public ForwardRestApi(AppProperties properties, Func<SerializableRequest, SerializableResponse> handler,params string[]paths) : base( handler,paths)
        {
            _properties = properties;
        }

        protected SerializableResponse RemoteRequest(String realUrl, SerializableRequest sr)
        {
            if (RequestData != null)
            {
                sr.RealUrl = realUrl;
                return RequestData(realUrl, sr);
            }
            if (sr.QueryParams.ContainsKey("runlocal"))
            {
                throw new Exception();
            }

            if (sr.Log)
            {
                sr.RealUrl = realUrl;
                var path = sr.ToLocalPath() + "\\req.json";
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllText(path, JsonConvert.SerializeObject(sr));
            }
            //throw new Exception();
            var res = new SerializableResponse();
            var method = HttpMethod.Get;
            if (sr.Method == "GET") method = HttpMethod.Get;
            if (sr.Method == "POST") method = HttpMethod.Post;
            if (sr.Method == "PUT") method = HttpMethod.Put;
            if (sr.Method == "DELETE") method = HttpMethod.Delete;
            var qp = "";
            if (sr.QueryParams.Any())
            {
                qp = "?" + string.Join("&", sr.QueryParams.Select(kvp => kvp.Key + "=" + kvp.Value));
            }

            var requestMessage = new HttpRequestMessage(method, realUrl + qp);
            foreach (var reqh in sr.Headers)
            {
                if (reqh.Key == "Host")
                {
                    var uri = new Uri(realUrl);
                    requestMessage.Headers.Add(reqh.Key, uri.Host);
                }
                else
                {
                    requestMessage.Headers.Add(reqh.Key, reqh.Value);
                }
            }
            if (sr.Method == "POST" || sr.Method == "PUT")
            {
                requestMessage.Content = new ByteArrayContent(sr.Content);
            }
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMilliseconds(300);
            var result = client.SendAsync(requestMessage);
            result.Wait();
            var resh = result.Result;



            res.Headers["Date"] = DateTime.Now.ToString("r");
            res.Headers["Last-Modified"] = DateTime.Now.ToString("r");
            var rb = resh.Content.ReadAsByteArrayAsync();
            rb.Wait();
            res.Content = rb.Result;
            try
            {
                res.ContentType = resh.Content.Headers.ContentType.MediaType;
            }
            catch (Exception)
            {

            }
            if (sr.Log)
            {
                res.RealUrl = realUrl;
                var path = sr.ToLocalPath() + "\\res.json";
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllText(path, JsonConvert.SerializeObject(res));
            }
            return res;
        }
    }
}
