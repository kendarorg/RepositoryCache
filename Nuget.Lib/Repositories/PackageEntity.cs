using System;
using NugetProtocol;
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
        public string FullVersion { get; set; }
        public string Authors { get; set; }
        public string Title { get; set; }
        public string Tags { get; set; }
        public string Summary { get; set; }
        public string Serviceable { get; set; }
        public bool RequireLicenseAcceptance { get; set; }
        public RepositoryXml Repository { get; set; }
        public string ReleaseNotes { get; set; }
        public string ProjectUrl { get; set; }
        public string Owners { get; set; }
        public string MinClientVersion { get; set; }
        public string LicenseUrl { get; set; }
        public string Language { get; set; }
        public string IconUrl { get; set; }
        public string Description { get; set; }
        public string Copyright { get; set; }
        public int TotalDownloads { get; set; }
        public bool Verified { get; set; }
    }
}