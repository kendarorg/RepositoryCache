using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Repositories
{
    public class QueryEntity : BaseEntity
    {
        public Guid RepositoryId { get; set; }
        public string FreeText { get; set; }
        public string PackageId { get; set; }
        public string Version { get; set; }
        public string PreVersion { get; set; }
        public string Title { get; set; }
        public string Tags { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public string Owner { get; set; }
        public bool HasRelease { get; set; }
        public bool HasPreRelease { get; set; }
        public Guid CommitId { get; set; }
        public DateTime CommitTimestamp { get; set; }
        public string IconUrl { get;  set; }
        public string LicenseUrl { get;  set; }
        public string ProjectUrl { get;  set; }
        public bool Verified { get;  set; }
        public bool Listed { get; set; }
        public bool PreListed { get; set; }
        public long TotalDownloads { get; set; }
        public string PreCsvVersions { get;  set; }
        public string CsvVersions { get;  set; }
        /*
public int Major { get; set; }
public int Minor { get; set; }
public int Patch { get; set; }
public string PreRelease { get; set; }
public string BuildMetadata { get; set; }

public int PreMajor { get; set; }
public int PreMinor { get; set; }
public int PrePatch { get; set; }
public string PrePreRelease { get; set; }
public string PreBuildMetadata { get; set; }*/

    }
}
