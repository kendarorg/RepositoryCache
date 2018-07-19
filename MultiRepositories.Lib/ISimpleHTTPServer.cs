using Ioc;
using MultiRepositories.Service;
using System.Collections.Generic;

namespace MultiRepositories
{
    public interface ISimpleHTTPServer : ISingleton
    {
        int Port { get; }

        void RegisterApi(RestAPI api);
        void Stop();
        void Start(
            string path, int port, bool logRequests, IEnumerable<string> urls, IEnumerable<string> ignores);
    }
}