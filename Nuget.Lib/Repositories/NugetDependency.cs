using Repositories;

namespace Nuget.Repositories
{
    public class NugetDependency : BaseEntity
    {
        public string OwnerPackageId { get; set; }
        public string OwnerVersion { get; set; }
        public string PackageId { get; set; }
        public string Range { get; set; }
        public string TargetFramework { get; set; }
    }
}