using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiRepositories.Service
{
    public abstract class RestAPI
    {
        private string[] _realPath;
        private Func<SerializableRequest, SerializableResponse> _handler;

        protected void SetHandler(Func<SerializableRequest, SerializableResponse> handler)
        {
            _handler = handler;
        }

        public RestAPI(String path, Func<SerializableRequest, SerializableResponse> handler)
        {
            _realPath = path.TrimStart('/').Split('/');
            _handler = handler;
        }

        public bool CanHandleRequest(String url)
        {
            var splittedUrl = url.TrimStart('/').Split('/');
            if (splittedUrl.Length != _realPath.Length) return false;
            for (int i = 0; i < splittedUrl.Length; i++)
            {
                var spl = splittedUrl[i];
                var mtc = _realPath[i];
                if (string.Compare(spl, mtc, true) == 0) continue;
                var start = mtc.IndexOf("{");
                var end = mtc.IndexOf("}");
                if (start >= 0 && end > start) continue;
                return false;
            }

            return true;
        }

        public SerializableResponse HandleRequest(SerializableRequest request)
        {
            var res = new Dictionary<string, string>();
            var splittedUrl = request.Url.TrimStart('/').Split('/');
            if (splittedUrl.Length != _realPath.Length)
            {
                throw new Exception();
            }
            for (int i = 0; i < splittedUrl.Length; i++)
            {
                var spl = splittedUrl[i];
                var mtc = _realPath[i];
                if (string.Compare(spl, mtc, true) == 0)
                {
                    continue;
                }
                var start = mtc.IndexOf("{");
                var end = mtc.IndexOf("}");
                var pre = start > 0 ? mtc.Substring(0, start) : "";
                var post = mtc.Substring(end+1);
                if (spl.StartsWith(pre) && spl.EndsWith(post))
                {
                    if (pre.Length > 0)
                    {
                        spl = spl.Substring(pre.Length);
                        mtc = mtc.Substring(pre.Length);
                    }
                    if (post.Length > 0)
                    {
                        spl = spl.Substring(0, spl.Length - post.Length);
                        mtc = mtc.Substring(0, mtc.Length - post.Length);
                    }
                    res.Add(mtc.Trim('{', '}'), spl);
                    continue;
                }

                throw new Exception();
            }
            request.PathParams = res;
            return _handler(request);
        }

        protected SerializableResponse JsonResponse(Object data)
        {
            var sr = new SerializableResponse
            {
                ContentType = "application/json"
            };
            sr.Headers["Date"] = DateTime.Now.ToString("r");
            sr.Headers["Last-Modified"] = DateTime.Now.ToString("r");
            sr.Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            return sr;
        }

        protected SerializableResponse JsonResponseString(String data)
        {
            var sr = new SerializableResponse
            {
                ContentType = "application/json"
            };
            sr.Headers["Date"] = DateTime.Now.ToString("r");
            sr.Headers["Last-Modified"] = DateTime.Now.ToString("r");
            sr.Content = Encoding.UTF8.GetBytes(data);
            return sr;
        }
        
    }
}
