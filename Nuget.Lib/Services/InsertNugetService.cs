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

namespace Nuget.Services
{
    public class InsertNugetService : IInsertNugetService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private readonly IQueryRepository _queryRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IPackagesRepository _packagesRepository;
        private readonly IPackagesStorage _packagesStorage;
        private readonly INugetDependenciesRepository _nugetDependencies;
        private readonly INugetAssembliesRepository _nugetAssemblies;

        public InsertNugetService(
            ITransactionManager transactionManager,
            IQueryRepository queryRepository,
            IRegistrationRepository registrationRepository,
            IPackagesRepository packagesRepository,
            IPackagesStorage packagesStorage,
            INugetDependenciesRepository nugetDependencies,
            INugetAssembliesRepository nugetAssemblies,
            IRepositoryEntitiesRepository repositoryEntitiesRepository)
        {
            _transactionManager = transactionManager;
            _repositoryEntitiesRepository = repositoryEntitiesRepository;
            this._queryRepository = queryRepository;
            this._registrationRepository = registrationRepository;
            this._packagesRepository = packagesRepository;
            this._packagesStorage = packagesStorage;
            this._nugetDependencies = nugetDependencies;
            this._nugetAssemblies = nugetAssemblies;
        }

        public PackageXml Deserialize(byte[] data, out DateTime timestamp)
        {
            timestamp = DateTime.MinValue;
            using (ZipFile zip = ZipFile.Read(new MemoryStream(data)))
            {
                foreach (var entry in zip)
                {
                    if (entry.FileName.ToLowerInvariant().EndsWith(".nuspec"))
                    {
                        timestamp = entry.CreationTime;
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
                            return (PackageXml)serializer.Deserialize(reader);
                        }
                    }
                }
            }
            return null;
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


            var data = new InsertData
            {
                CommitId = Guid.NewGuid(),
                Nuspec = Deserialize(content, out commitTimestamp),
                HashKey = CalculateSha512(content),
                HashAlgorithm = "SHA512",
                Size = content.Length,
                RepoId = repoId
            };
            data.Id = data.Nuspec.Metadata.Id;
            data.Version = SemVer.SemVersion.Parse(data.Nuspec.Metadata.Version).ToNormalizedVersion();
            data.OriginalVersion = SemVer.SemVersion.Parse(data.Nuspec.Metadata.Version).ToString();
            data.Timestamp = commitTimestamp;

            using (var transaction = _transactionManager.BeginTransaction())
            {
                try
                {
                    InsertPackagesStorage(data, content);
                    InsertPackages(data, transaction);
                    InsertQuery(data, transaction);
                    InsertRegistration(data, transaction);
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

        public void InsertQuery(InsertData data, ITransaction transaction)
        {
            throw new NotImplementedException();
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
                        PackageId = asm.Id,
                        Range = asm.Version,
                        TargetFramework = null,
                        OwnerPackageId = data.Id,
                        OwnerVersion = data.Version
                    }, transaction);
                }
            }
            if (metadata.Dependencies != null &&
                metadata.Dependencies.Group != null &&
               metadata.Dependencies.Group.Count > 0)
            {
                foreach (var group in metadata.Dependencies.Group)
                {
                    var targetFw = string.IsNullOrWhiteSpace(group.TargetFramework) ? null : group.TargetFramework;
                    foreach (var asm in group.Dependency)
                    {
                        _nugetDependencies.Save(new NugetDependency
                        {
                            PackageId = asm.Id,
                            Range = asm.Version,
                            TargetFramework = targetFw,
                            OwnerPackageId = data.Id,
                            OwnerVersion = data.Version
                        }, transaction);
                    }
                }
            }
            throw new NotImplementedException();
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
                    AssemblyName = asm.AssemblyName,
                    TargetFramework = string.IsNullOrWhiteSpace(asm.TargetFramework) ? null : asm.TargetFramework,
                    OwnerPackageId = data.Id,
                    OwnerVersion = data.Version
                }, transaction);
            }
            throw new NotImplementedException();
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
            p.HashKey = data.HashKey;
            p.Nuspec = data.Nuspec.ToString();
            p.PackageId = data.Id;
            p.PackageIdAndVersion = data.Id + "." + data.Version;
            p.RepositoryId = data.RepoId;
            p.Size = data.Size;
            p.Version = data.Version;

            //WARNING new since here
            p.Authors = metadata.Authors;
            p.Copyright = metadata.Copyright;
            p.Description = metadata.Description;
            p.IconUrl = metadata.IconUrl;
            p.Language = metadata.Language;
            p.LicenseUrl = metadata.LicenseUrl;
            p.MinClientVersion = metadata.MinClientVersion;
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

            if (isNew)
            {
                _packagesRepository.Save(p, transaction);
            }
            else
            {
                _packagesRepository.Update(p, transaction);
            }
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
            r.CommitId = data.CommitId;
            r.CommitTimestamp = data.Timestamp;
            r.Major
        }
    }
}
