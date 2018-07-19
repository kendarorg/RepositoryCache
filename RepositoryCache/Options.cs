using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryCache

{
    public class Options
    {
        [Option("port", Default = 9080, HelpText = "Port.")]
        public int Port { get; set; }
        [Option("host", Default = "localhot", HelpText = "Host.")]
        public string Host { get; set; }
        [Option("logrequests", Default = false, HelpText = "Log requests.")]
        public bool LogRequests { get; set; }
        [Option("showintray", Default = false, HelpText = "Log requests.")]
        public bool ShowInTray { get; set; }
        [Option("path", Default = "", HelpText = "Root to service.")]
        public string Path { get; set; }
        [Option("urls", Default = null, HelpText = "Urls to proxy.")]
        public IEnumerable<string> Urls { get; set; }
        [Option("ignore", Default = null, HelpText = "Urls to ignore.")]
        public IEnumerable<string> Ignores { get; set; }
        [Option("settings", Default = null, HelpText = "Settings file.")]
        public string Settings { get; set; }
    }
}
