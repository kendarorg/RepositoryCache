using System;
using MultiRepositories.Service;

namespace MultiRepositories
{
    public class MockRestApi : RestAPI
    {
        public MockRestApi(Func<SerializableRequest, SerializableResponse> handler, params string[] paths) : base(handler, paths)
        {
            _regexes = new System.Collections.Generic.Dictionary<string, System.Text.RegularExpressions.Regex>(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}