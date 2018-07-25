using Repositories;
using System;

namespace Nuget.Repositories
{
    //WARNING: Can be target framework only!!
    public class NugetDependency : BaseEntity
    {
        public string OwnerPackageId { get; set; }
        public string OwnerVersion { get; set; }
        public string PackageId { get; set; }
        public string Range { get; set; }
        public string TargetFramework { get; set; }
        public Guid RepositoryId { get; set; }
    }
}