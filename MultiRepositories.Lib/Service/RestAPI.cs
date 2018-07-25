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
        private List<string[]> _realPaths;
        private Func<SerializableRequest, SerializableResponse> _handler;

        protected void SetHandler(Func<SerializableRequest, SerializableResponse> handler)
        {
            _handler = handler;
        }

        public RestAPI( Func<SerializableRequest, SerializableResponse> handler,params string[]paths)
        {
            _realPaths = new List<string[]>();
            foreach (var path in paths)
            {
                _realPaths.Add(path.TrimStart('/').Split('/'));
            }
            _handler = handler;
        }

        public bool CanHandleRequest(String url)
        {
            var splittedUrl = url.TrimStart('/').Split('/');
            
            foreach (var realPath in _realPaths)
            {
                var isGood = true;
                if (splittedUrl.Length != realPath.Length) continue;
                for (int i = 0; i < splittedUrl.Length; i++)
                {
                    var spl = splittedUrl[i];
                    var mtc = realPath[i];
                    if (string.Compare(spl, mtc, true) == 0) continue;
                    var start = mtc.IndexOf("{");
                    var end = mtc.IndexOf("}");
                    if (start >= 0 && end > start) continue;
                    isGood = false;
                    break;
                }
                if (isGood) return true;
            }
            return false;
        }

        public SerializableResponse HandleRequest(SerializableRequest request)
        {
            var res = new Dictionary<string, string>();
            var splittedUrl = request.Url.TrimStart('/').Split('/');
            foreach (var realPath in _realPaths)
            {
                var isGood = true;
                if (splittedUrl.Length != realPath.Length)
                {
                    continue;
                }
                for (int i = 0; i < splittedUrl.Length; i++)
                {
                    var spl = splittedUrl[i];
                    var mtc = realPath[i];
                    if (string.Compare(spl, mtc, true) == 0)
                    {
                        continue;
                    }
                    var start = mtc.IndexOf("{");
                    var end = mtc.IndexOf("}");
                    var pre = start > 0 ? mtc.Substring(0, start) : "";
                    var post = mtc.Substring(end + 1);
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
                    isGood = false;
                    break;
                }
                if (isGood)
                {
                    request.PathParams = res;
                    return _handler(request);
                }
            }
            throw new Exception();
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
