using NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NugetIntegration
{
    class Program
    {
        static public string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        static void Main(string[] args)
        {
            var localRepo = PackageRepositoryFactory.Default.CreateRepository(@"locationOfLocalPackage");
            var package = localRepo.FindPackagesById("packageId").First();
            var packageFile = new FileInfo(@"packagePath");
            var size = packageFile.Length;
            var ps = new PackageServer("http://localhost:", "userAgent");
            ps.PushPackage("MYAPIKEY", package, size, 1800, false);
        }
    }
}
