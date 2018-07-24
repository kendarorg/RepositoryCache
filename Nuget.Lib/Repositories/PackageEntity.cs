using System;
using Repositories;

namespace Nuget.Repositories
{
    public class PackageEntity : BaseEntity
    {
        public string Nuspec { get; set; }
        public string PackageIdAndVersion { get; set; }
        public string PackageId { get; set; }
        public string Version { get; set; }
        public Guid RepositoryId { get; set; }
        public DateTime CommitTimestamp { get;  set; }
        public Guid CommitId { get; set; }
        public string HashKey { get; set; }
        public string HashAlgorithm { get; set; }
        public int Size { get; set; }
    }
}