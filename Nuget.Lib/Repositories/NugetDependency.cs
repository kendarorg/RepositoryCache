using Repositories;

namespace Nuget.Repositories
{
    public class NugetDependency : BaseEntity
    {
        public string PackageId { get; internal set; }
        public string Version { get; internal set; }
    }
}