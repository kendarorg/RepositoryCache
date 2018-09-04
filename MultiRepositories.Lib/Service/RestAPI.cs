using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MultiRepositories.Service
{
    public class Request
    {
        public Request()
        {
            Methods = new List<string>();
        }
        public List<string> Methods { get; set; }
        public string[] Path { get; set; }
    }
    public abstract class RestAPI
    {
        private List<Request> _realPaths;
        private Dictionary<String, Regex> _realPathsRegex;
        private Func<SerializableRequest, SerializableResponse> _handler;

        protected void SetHandler(Func<SerializableRequest, SerializableResponse> handler)
        {
            _handler = handler;
        }

        public RestAPI(Func<SerializableRequest, SerializableResponse> handler, params string[] paths)
        {
            _realPathsRegex = new Dictionary<string, Regex>();
            _realPaths = new List<Request>();
            Request lastRequest = new Request();
            foreach (var path in paths)
            {
                if (path.StartsWith("*"))
                {
                    lastRequest.Methods.Add(path.Trim('*'));
                }
                else
                {
                    var cleaned = path.TrimStart('/');

                    if (cleaned.StartsWith("^"))
                    {
                        lastRequest.Path = new string[] { cleaned };
                        _realPathsRegex[cleaned] = new Regex(cleaned, RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
                    }
                    else
                    {
                        lastRequest.Path = cleaned.Split('/');
                    }
                    _realPaths.Add(lastRequest);
                    lastRequest = new Request();
                }

            }
            _handler = handler;
        }

        public void Append(Dictionary<string, string> dic, string key, string data)
        {
            //key = key.Trim('*');
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
            var cleaned = url.Trim('/');
            if (realPath.Length == 1 && realPath[0].StartsWith("^"))
            {
                var match = _realPathsRegex[realPath[0]].Match(cleaned);
                if (!match.Success) return null;
                var datareg = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                foreach (string groupName in _realPathsRegex[realPath[0]].GetGroupNames())
                {
                    if (match.Groups[groupName].Success)
                    {
                        datareg[groupName] = match.Groups[groupName].Value;
                    };
                }
                return datareg;
            }

            var splittedUrl = cleaned.Split('/');

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
                if (splittedIndex >= splittedUrl.Length)
                {
                    return null;
                }
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
                            Append(data,  index, splittedUrl[splittedIndex]);
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
                                if (mtc.IndexOf("#") > 0)
                                {
                                    if (!AppendMerge(data, mtc.Trim('{', '}'), spl))
                                    {
                                        return null;
                                    }
                                }
                                else
                                {
                                    Append(data, mtc.Trim('{', '}'), spl);
                                }
                                limitSplit--;
                                continue;
                            }
                            return null;
                        }
                        else if (string.Compare(spl, mtc, true) == 0)
                        {
                            limitSplit--;
                            continue;
                        }
                        return null;
                    }
                    for (; splittedIndex <= limitSplit; splittedIndex++)
                    {
                        spl = splittedUrl[splittedIndex];
                        Append(data, index, spl);
                    }
                    return data;
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

                        if (mtc.IndexOf("#") > 0)
                        {
                            if (!AppendMerge(data, mtc.Trim('{', '}'), spl))
                            {
                                return null;
                            }
                        }
                        else
                        {
                            Append(data, mtc.Trim('{', '}'), spl);
                        }
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

        protected static Dictionary<string, Regex> _regexes = new Dictionary<string, Regex>(StringComparer.InvariantCultureIgnoreCase);
        //https://docs.oracle.com/javase/tutorial/java/package/namingpkgs.html
        private bool AppendMerge(Dictionary<string, string> data, string mtc, string spl)
        {
            var trimmed = mtc.Trim('{', '}');
            var sharp = trimmed.IndexOf('#');
            var regex = trimmed.Substring(sharp + 1);
            if (!_regexes.ContainsKey(regex))
            {
                _regexes[regex] = new Regex(regex, RegexOptions.CultureInvariant | RegexOptions.Compiled |
                    RegexOptions.ExplicitCapture);
            }
            var match = _regexes[regex].Match(spl);
            if (!match.Success) return false;
            data[trimmed.Substring(0, sharp)] = spl;
            foreach (string groupName in _regexes[regex].GetGroupNames())
            {
                if (match.Groups[groupName].Success)
                {
                    data[groupName] = match.Groups[groupName].Value;
                };
            }
            return true;
        }

        public bool CanHandleRequest(String url, string method = null)
        {
            foreach (var realPath in _realPaths)
            {
                if (VerifyHttpMethod(method, realPath))
                {
                    var res = BuildPath(url, realPath.Path);
                    if (res != null) return true;
                }
            }
            return false;
            //return OldCanHandleRequest(url);
        }

        private static bool VerifyHttpMethod(string method, Request realPath)
        {
            return method == null || realPath.Methods.Count == 0 || realPath.Methods.Any(a => string.Compare(a, method, true) == 0);
        }

        public SerializableResponse HandleRequest(SerializableRequest request)
        {
            foreach (var realPath in _realPaths)
            {
                if (VerifyHttpMethod(request.Method, realPath))
                {
                    var res = BuildPath(request.Url, realPath.Path);
                    if (res != null)
                    {
                        request.PathParams = res;
                        return _handler(request);
                    }
                }
            }

            throw new Exception();

            //return OldHandleRequest(request);
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
