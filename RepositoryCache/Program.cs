using MultiRepositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using Newtonsoft.Json;
using MultiRepositories.Repositories;

namespace RepositoryCache
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts))
                .WithNotParsed<Options>((errs) => HandleParseError(errs));
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            
        }

        private static void RunOptionsAndReturnExitCode(Options opts)
        {
            var settingsFile = opts.Settings;
            
            if (!string.IsNullOrWhiteSpace(settingsFile) && File.Exists(settingsFile))
            {
                Console.WriteLine("Reading settings from " + settingsFile);
                opts = JsonConvert.DeserializeObject<Options>(File.ReadAllText(settingsFile));
            }
            if (string.IsNullOrWhiteSpace(opts.Path))
            {
                opts.Path = Directory.GetCurrentDirectory();
            }
            opts.Settings = settingsFile;
            
            var settings = JsonConvert.SerializeObject(opts);
            Console.WriteLine(settings);
            var applicationProperties = new AppProperties();
            var availableRepositories = new AvailableRepositoriesRepository(applicationProperties);

            var shhtp = new SimpleHTTPServer(applicationProperties,availableRepositories,
                opts.Path, opts.Port,opts.LogRequests,opts.Urls,opts.Ignores);

            while (Console.ReadKey().KeyChar != 'q')
            {
                Console.WriteLine("");
                continue;
            }
            shhtp.Stop();
        }
    }
}
