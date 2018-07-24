using Repositories;

namespace Nuget.Repositories
{
    public class NugetAssemblyGroup : BaseEntity
    {
        public string PackageId { get; set; }
        public string Version { get; set; }
        public string TargetFramework { get; internal set; }
        public string AssemblyName { get; internal set; }
    }
}