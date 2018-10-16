using Ioc;
using Ionic.Zip;
using MultiRepositories.Repositories;
using Nuget.Repositories;
using NugetProtocol;
using Repositories;
using SemVer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Nuget.Framework;
using Nuget.Framework.FromNugetTools;
using NuGet.Frameworks;

namespace Nuget.Services
{
    public class InsertNugetService : IInsertNugetService, ISingleton
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IQueryRepository _queryRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IPackagesRepository _packagesRepository;
        private readonly IPackagesStorage _packagesStorage;
        private readonly INugetDependenciesRepository _nugetDependencies;
        private readonly INugetAssembliesRepository _nugetAssemblies;
        private IFrameworkChecker _frameworkChecker;

        public InsertNugetService(
            IFrameworkChecker frameworkChecker,
            ITransactionManager transactionManager,
            IQueryRepository queryRepository,
            IRegistrationRepository registrationRepository,
            IPackagesRepository packagesRepository,
            IPackagesStorage packagesStorage,
            INugetDependenciesRepository nugetDependencies,
            INugetAssembliesRepository nugetAssemblies,
            IRepositoryEntitiesRepository repositoryEntitiesRepository)
        {
            _frameworkChecker = frameworkChecker;
            _transactionManager = transactionManager;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._queryRepository = queryRepository;
            this._registrationRepository = registrationRepository;
            this._packagesRepository = packagesRepository;
            this._packagesStorage = packagesStorage;
            this._nugetDependencies = nugetDependencies;
            this._nugetAssemblies = nugetAssemblies;
        }
        
        public DeserializedPackage Deserialize(byte[] data, out DateTime timestamp)
        {
            var result = new DeserializedPackage();
            timestamp = DateTime.MinValue;
            using (ZipFile zip = ZipFile.Read(new MemoryStream(data)))
            {
                foreach (var entry in zip)
                {
                    if (entry.FileName.ToLowerInvariant().StartsWith("lib/"))
                    {
                        var splitted = entry.FileName.Split('/');
                        if (splitted.Length > 1)
                        {
                            if (_frameworkChecker.FrameworkExists(splitted[1]))
                            {
                                var shortFwName = _frameworkChecker.GetShortFolderName(splitted[1]);
                                if (result.Frameworks.All(f => f != shortFwName))
                                {
                                    result.Frameworks.Add(shortFwName);
                                }
                            }
                        }
                    }
                    if (entry.FileName.ToLowerInvariant().EndsWith(".nuspec"))
                    {
                        timestamp = entry.ModifiedTime > entry.LastModified ? entry.ModifiedTime : entry.LastModified;
                        var ms = new MemoryStream();
                        entry.Extract(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        var ss = ms.ToArray();
                        var fileName = entry.FileName;
                        var xml = Encoding.UTF8.GetString(ss);
                        string pattern = @"xmlns=""[a-zA-Z0-9:\/._]{1,}""";
                        System.Text.RegularExpressions.Match m = Regex.Match(xml, pattern);
                        if (m.Success)
                        {
                            xml = xml.Replace(m.Value, "");
                        }
                        var msx = new MemoryStream(Encoding.UTF8.GetBytes(xml));
                        msx.Seek(0, SeekOrigin.Begin);
                        var serializer = new XmlSerializer(typeof(PackageXml));

                        using (var reader = XmlReader.Create(msx))
                        {
                            result.Nuspec = (PackageXml) serializer.Deserialize(reader);
                            if (result.Nuspec.Metadata.FrameworkAssemblies != null)
                            {
                                if (result.Nuspec.Metadata.FrameworkAssemblies.FrameworkAssembly != null)
                                {
                                    foreach (var item in result.Nuspec.Metadata.FrameworkAssemblies.FrameworkAssembly)
                                    {
                                        foreach (var tf in item.TargetFramework.Split(',').Select(a => a.Trim()))
                                        {
                                            if(string.IsNullOrWhiteSpace(tf))continue;
                                            var shortFwName =
                                                _frameworkChecker.GetShortFolderName(tf);
                                            if (shortFwName == null)
                                            {
                                                var tt  = NuGetFramework.Parse(tf);
                                                var nn = new Nuget.Framework.FromNuget.NuGetFramework(tt.Framework,
                                                    tt.Version, tt.Profile);
                                                _frameworkChecker.Add(nn,tf);
                                                shortFwName = nn.GetShortFolderName();

                                            }
                                            if (result.Frameworks.All(f => f != shortFwName))
                                            {
                                                result.Frameworks.Add(shortFwName);
                                            }
                                        }
                                    }
                                }

                                if (result.Nuspec.Metadata.Dependencies?.Group != null)
                                {
                                    foreach (var item in result.Nuspec.Metadata.Dependencies.Group)
                                    {
                                        if (!string.IsNullOrWhiteSpace(item.TargetFramework))
                                        {
                                            foreach (var tf in item.TargetFramework.Split(',').Select(a => a.Trim()))
                                            {
                                                if (string.IsNullOrWhiteSpace(tf)) continue;
                                                var shortFwName =
                                                    _frameworkChecker.GetShortFolderName(tf);
                                                if (shortFwName == null)
                                                {
                                                    var tt = NuGetFramework.Parse(tf);
                                                    var nn = new Nuget.Framework.FromNuget.NuGetFramework(tt.Framework,
                                                        tt.Version, tt.Profile);
                                                    _frameworkChecker.Add(nn, tf);
                                                    shortFwName = nn.GetShortFolderName();

                                                }
                                                if (result.Frameworks.All(f => f != shortFwName))
                                                {
                                                    result.Frameworks.Add(shortFwName);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            return result.Nuspec==null?null:result;
        }

        public string CalculateSha512(byte[] bytes)
        {
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        public void Insert(Guid repoId, string nugetApiKey, byte[] content)
        {
            //The commit timestamp for package should be taken by the nuspec file timestamp!
            var commitTimestamp = DateTime.UtcNow;

            var packageContent = Deserialize(content, out commitTimestamp);
            
            var data = new InsertData
            {
                CommitId = Guid.NewGuid(),
                Nuspec = packageContent.Nuspec,
                Frameworks = packageContent.Frameworks.Select(a=>a.Trim()).Where(b=>!string.IsNullOrWhiteSpace(b)).ToList(),
                HashKey = CalculateSha512(content),
                HashAlgorithm = "SHA512",
                Size = content.Length,
                RepoId = repoId,
            };
            data.Id = data.Nuspec.Metadata.Id.ToLowerInvariant();
            data.Version = SemVer.SemVersion.Parse(data.Nuspec.Metadata.Version.ToLowerInvariant()).ToNormalizedVersion().ToLowerInvariant();
            data.OriginalVersion = SemVer.SemVersion.Parse(data.Nuspec.Metadata.Version.ToLowerInvariant()).ToString().ToLowerInvariant();
            data.Timestamp = commitTimestamp;

            using (var transaction = _transactionManager.BeginTransaction())
            {
                try
                {
                    InsertPackagesStorage(data, content);
                    InsertRegistration(data, transaction);
                    InsertPackages(data, transaction);
                    InsertQuery(data, transaction);
                    InsertDependencies(data, transaction);
                    InsertAssemblies(data, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    transaction.Rollback();
                }
            }
        }

        public void InsertDependencies(InsertData data, ITransaction transaction)
        {
            foreach (var item in _nugetDependencies.GetDependencies(data.RepoId, data.Id, data.Version).ToList())
            {
                _nugetDependencies.Delete(item.Id, transaction);
            }

            var metadata = data.Nuspec.Metadata;
            if (metadata.Dependencies != null &&
                metadata.Dependencies.Dependency != null &&
               metadata.Dependencies.Dependency.Count > 0)
            {
                foreach (var asm in metadata.Dependencies.Dependency)
                {
                    _nugetDependencies.Save(new NugetDependency
                    {
                        RepositoryId = data.RepoId,
                        PackageId = asm.Id,
                        Range = asm.Version,
                        TargetFramework = null,
                        OwnerPackageId = data.Id.ToLowerInvariant(),
                        OwnerVersion = data.Version.ToLowerInvariant()
                    }, transaction);
                }
            }
            if (metadata.Dependencies != null &&
                metadata.Dependencies.Group != null &&
               metadata.Dependencies.Group.Count > 0)
            {
                foreach (var group in metadata.Dependencies.Group)
                {
                    var targetFw = string.IsNullOrWhiteSpace(group.TargetFramework) ? null : group.TargetFramework.ToLowerInvariant();
                    foreach (var asm in group.Dependency)
                    {
                        _nugetDependencies.Save(new NugetDependency
                        {
                            PackageId = asm.Id,
                            Range = asm.Version,
                            TargetFramework = targetFw,
                            OwnerPackageId = data.Id.ToLowerInvariant(),
                            OwnerVersion = data.Version.ToLowerInvariant()
                        }, transaction);
                    }
                }
            }
        }

        public void InsertAssemblies(InsertData data, ITransaction transaction)
        {

            foreach (var item in _nugetAssemblies.GetGroups(data.RepoId, data.Id, data.Version).ToList())
            {
                _nugetAssemblies.Delete(item.Id, transaction);
            }

            var metadata = data.Nuspec.Metadata;
            if (metadata.FrameworkAssemblies == null ||
                metadata.FrameworkAssemblies.FrameworkAssembly == null ||
               metadata.FrameworkAssemblies.FrameworkAssembly.Count == 0)
            {
                return;
            }

            foreach (var asm in metadata.FrameworkAssemblies.FrameworkAssembly)
            {
                _nugetAssemblies.Save(new NugetAssemblyGroup
                {
                    RepositoryId = data.RepoId,
                    AssemblyName = asm.AssemblyName,
                    TargetFramework = string.IsNullOrWhiteSpace(asm.TargetFramework) ? null : asm.TargetFramework.ToLowerInvariant(),
                    OwnerPackageId = data.Id.ToLowerInvariant(),
                    OwnerVersion = data.Version.ToLowerInvariant()
                }, transaction);
            }
        }

        public void InsertPackages(InsertData data, ITransaction transaction)
        {
            var p = _packagesRepository.GetByPackage(data.RepoId, data.Id, data.Version);
            var isNew = p == null;
            if (!isNew)
            {
                var oldVersion = SemVersion.Parse(p.FullVersion);
                var newVersion = SemVersion.Parse(data.OriginalVersion);
                if (oldVersion.Build != null && newVersion.Build != null && oldVersion.Build.ToLowerInvariant() == newVersion.Build.ToLowerInvariant())
                {
                    throw new Exception("DUPLICATE PACKAGE");
                }
                if (oldVersion.Build == newVersion.Build)
                {
                    throw new Exception("DUPLICATE PACKAGE");
                }
            }
            else
            {
                p = new PackageEntity();
            }
            var metadata = data.Nuspec.Metadata;
            p.CommitId = data.CommitId;
            p.CommitTimestamp = data.Timestamp;
            p.FullVersion = data.OriginalVersion;
            p.HashAlgorithm = data.HashAlgorithm;
            p.HashKey = data.HashKey.ToUpperInvariant();
            p.Nuspec = data.Nuspec.ToString();
            p.PackageId = data.Id.ToLowerInvariant();
            p.PackageIdAndVersion = (data.Id + "." + data.Version).ToLowerInvariant();
            p.RepositoryId = data.RepoId;
            p.Size = data.Size;
            p.Version = data.Version.ToLowerInvariant();

            //WARNING new since here
            p.Authors = metadata.Authors;
            p.Copyright = metadata.Copyright;
            p.Description = metadata.Description;
            p.IconUrl = metadata.IconUrl;
            if (!string.IsNullOrWhiteSpace(metadata.Language))
            {
                p.Language = metadata.Language.ToUpperInvariant();
            }
            p.LicenseUrl = metadata.LicenseUrl;
            if (!string.IsNullOrWhiteSpace(metadata.MinClientVersion))
            {
                p.MinClientVersion = metadata.MinClientVersion.ToLowerInvariant();
            }
            if (data.Frameworks.Any())
            {
                p.Frameworks = "|" + String.Join("|", data.Frameworks) + "|";
            }
            
            p.Owners = metadata.Owners;
            p.ProjectUrl = metadata.ProjectUrl;
            p.ReleaseNotes = metadata.ReleaseNotes;
            p.Repository = metadata.Repository;
            if (!string.IsNullOrWhiteSpace(metadata.RequireLicenseAcceptance))
            {
                p.RequireLicenseAcceptance = bool.Parse(metadata.RequireLicenseAcceptance);
            }
            p.Serviceable = metadata.Serviceable;
            p.Summary = metadata.Summary;
            p.Tags = metadata.Tags;
            p.Title = metadata.Title;
            p.Verified = data.Verified;
            p.RepositoryId = data.RepoId;
            _packagesRepository.Save(p, transaction);
        }

        public void InsertPackagesStorage(InsertData data, byte[] content)
        {
            var repo = _repositoryEntitiesRepository.GetById(data.RepoId);
            _packagesStorage.Save(repo, data.Id, data.Version, content);
        }

        public void InsertRegistration(InsertData data, ITransaction transaction)
        {
            var metadata = data.Nuspec.Metadata;
            var r = _registrationRepository.GetSpecific(data.RepoId, data.Id, data.Version);
            var isNew = r == null;
            if (isNew)
            {
                r = new RegistrationEntity();
            }
            var version = SemVersion.Parse(data.OriginalVersion);
            r.CommitId = data.CommitId;
            r.CommitTimestamp = data.Timestamp;
            r.PackageId = data.Id;
            r.Major = version.Major;
            r.Minor = version.Minor;
            r.Patch = version.Patch;
            r.PreRelease = version.Prerelease;
            if (data.Frameworks.Any())
            {
                r.Frameworks = "|" + String.Join("|", data.Frameworks) + "|";
            }
            
            r.Listed = true;
            r.RepositoryId = data.RepoId;
            if (version.Extra != null)
            {
                r.Extra = version.Extra;
            }
            r.Version = version.ToNormalizedVersion();
            _registrationRepository.Save(r, transaction);
        }

        public void InsertQuery(InsertData data, ITransaction transaction)
        {
            var metadata = data.Nuspec.Metadata;
            var p = _queryRepository.GetByPackage(data.RepoId, data.Id);
            SemVersion newVersion = SemVersion.Parse(metadata.Version);
            var isPre = !string.IsNullOrWhiteSpace(newVersion.Prerelease);
            var registrationEntities = _registrationRepository.GetAllByPackageId(data.RepoId, data.Id)
                        .OrderByDescending((a) => SemVersion.Parse(a.Version));

            var isNew = p == null;
            if (isNew)
            {
                p = new QueryEntity();
            }

            if (data.Frameworks.Any())
            {
                p.Frameworks = "|" + String.Join("|", data.Frameworks) + "|";
            }

            p.RepositoryId = data.RepoId;
            p.CommitId = data.CommitId;
            p.CommitTimestamp = data.Timestamp;
            p.PackageId = data.Id.ToLowerInvariant();
            p.RepositoryId = data.RepoId;
            p.Summary = metadata.Summary;
            if (!string.IsNullOrWhiteSpace(metadata.Tags))
            {
                p.Tags ="|"+string.Join("|", metadata.Tags.Split(' ',',').Where(a=>a.Trim().Length>0))+"|";
            }
            p.Title = metadata.Title;
            if (!string.IsNullOrWhiteSpace(metadata.Authors))
            {
                p.Author = "|" + string.Join("|", metadata.Authors.Split( ',').Where(a => a.Trim().Length > 0)) + "|";
            }
            p.Description = metadata.Description;
            if (!string.IsNullOrWhiteSpace(metadata.Owners))
            {
                p.Owner = "|" + string.Join("|", metadata.Owners.Split( ',').Where(a => a.Trim().Length > 0)) + "|";
            }

            p.IconUrl = metadata.IconUrl;
            p.LicenseUrl = metadata.LicenseUrl;
            p.ProjectUrl = metadata.ProjectUrl;
            p.Verified = data.Verified;

            if (isPre)
            {
                var lastVisible = registrationEntities.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.PreRelease) && a.Listed);
                p.PreVersion = null;
                p.PreListed = false;
                p.PreCsvVersions ="|"+ string.Join("|",registrationEntities.Where(a => !string.IsNullOrWhiteSpace(a.PreRelease) && a.Listed).
                    Select(a => a.Version))+"|";
                p.HasPreRelease = false;
                if (lastVisible != null)
                {
                    p.HasPreRelease = true;
                    p.PreVersion = lastVisible.Version;
                    p.PreListed = true;
                }
            }
            else
            {
                var lastVisible = registrationEntities.FirstOrDefault(a => string.IsNullOrWhiteSpace(a.PreRelease) && a.Listed);
                p.Version = null;
                p.Listed = false;
                p.CsvVersions = "|" + string.Join("|", registrationEntities.Where(a => string.IsNullOrWhiteSpace(a.PreRelease) && a.Listed).
                    Select(a => a.Version)) + "|";

                p.HasRelease = false;
                if (lastVisible != null)
                {
                    p.HasRelease = true;
                    p.Version = lastVisible.Version;
                    p.Listed = true;
                }
            }
            p.FreeText = p.Title + " " + p.Description + " " + p.Summary;
             _queryRepository.Save(p, transaction);
        }
    }
}
