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

        public RestAPI(Func<SerializableRequest, SerializableResponse> handler, params string[] paths)
        {
            _realPaths = new List<string[]>();
            foreach (var path in paths)
            {
                _realPaths.Add(path.TrimStart('/').Split('/'));
            }
            _handler = handler;
        }

        public void Append(Dictionary<string, string> dic, string key, string data)
        {
            if (!dic.ContainsKey(key))
            {
                dic[key] = data;
            }
            else
            {
                dic[key] += "/" + data;
            }

        }

        private Dictionary<string, string> BuildPath(string url, string[] realPath)
        {
            var splittedUrl = url.Trim('/').Split('/');
            var data = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var stars = realPath.Count(a => a.StartsWith("{*"));
            if (stars > 1)
            {
                throw new Exception("Invlid Url");
            }
            else if (stars == 0)
            {
                if (splittedUrl.Length != realPath.Length)
                {
                    return null;
                }
            }

            var realIndex = 0;
            var splittedIndex = 0;
            for (; realIndex < realPath.Length; realIndex++)
            {
                var mtc = realPath[realIndex];
                var spl = splittedUrl[splittedIndex];

                var start = mtc.IndexOf("{");
                var end = mtc.IndexOf("}");

                if (mtc.StartsWith("{*") && mtc.EndsWith("}"))
                {
                    var index = mtc.Substring(2, mtc.Length - 3);
                    //What is after is useless
                    if (realIndex == realPath.Length - 1)
                    {
                        for (; splittedIndex < splittedUrl.Length; splittedIndex++)
                        {
                            Append(data, "*" + index, splittedUrl[splittedIndex]);
                        }
                        return data;
                    }
                    var limitReal = realPath.Length - 1;
                    var limitSplit = splittedUrl.Length - 1;

                    for (; limitReal > realIndex; limitReal--)
                    {
                        mtc = realPath[limitReal];
                        spl = splittedUrl[limitSplit];

                        start = mtc.IndexOf("{");
                        end = mtc.IndexOf("}");
                        if (string.Compare(spl, mtc, true) == 0)
                        {
                            limitSplit--;
                            continue;
                        }
                        else if (start >= 0 && end > start)
                        {
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
                                Append(data, mtc.Trim('{', '}'), spl);
                                limitSplit--;
                                continue;
                            }
                            return null;
                        }
                    }
                    for (; splittedIndex <= limitSplit; splittedIndex++)
                    {
                        spl = splittedUrl[splittedIndex];
                        Append(data, "*" + index, spl);
                    }
                    return data;

                    /*var expected = realPath[realIndex + 1];
                    var indexFounded = false;
                    for (; splittedIndex < splittedUrl.Length; splittedIndex++)
                    {
                        if (splittedUrl[splittedIndex] == expected)
                        {
                            indexFounded = true;
                            splittedIndex--;
                            break;
                        }
                        Append(data, index, splittedUrl[splittedIndex]);
                    }
                    if (!indexFounded)
                    {
                        return null;
                    }*/
                }
                else if (string.Compare(spl, mtc, true) == 0)
                {
                    splittedIndex++;
                    continue;
                }
                else if (start >= 0 && end > start)
                {
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
                        Append(data, mtc.Trim('{', '}'), spl);
                        splittedIndex++;
                        continue;
                    }
                    return null;
                }
                else
                {
                    return null;
                }


            }
            return data;
        }

        public bool CanHandleRequest(String url)
        {
            foreach (var realPath in _realPaths)
            {
                var res = BuildPath(url, realPath);
                if (res != null) return true;
            }
            return false;
            //return OldCanHandleRequest(url);
        }

        private bool OldCanHandleRequest(string url)
        {
            var splittedUrl = url.Trim('/').Split('/');

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
            foreach (var realPath in _realPaths)
            {
                var res = BuildPath(request.Url, realPath);
                if (res != null)
                {
                    request.PathParams = res;
                    return _handler(request);
                }
            }

            throw new Exception();

            //return OldHandleRequest(request);
        }

        private SerializableResponse OldHandleRequest(SerializableRequest request)
        {
            var res = new Dictionary<string, string>();
            var splittedUrl = request.Url.Trim('/').Split('/');
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
