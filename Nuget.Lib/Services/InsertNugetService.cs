using Ioc;
using Ionic.Zip;
using Nuget.Repositories;
using NugetProtocol;
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
        private readonly IQueryRepository _queryRepository;
        private readonly IRegistrationRepository _registrationRepository;
        private readonly IPackagesRepository _packagesRepository;
        private readonly IPackagesStorage _packagesStorage;
        private readonly INugetDependenciesRepository _nugetDependencies;
        private readonly INugetAssembliesRepository _nugetAssemblies;

        public InsertNugetService(
            IQueryRepository queryRepository,
            IRegistrationRepository registrationRepository,
            IPackagesRepository packagesRepository,
            IPackagesStorage packagesStorage,
            INugetDependenciesRepository nugetDependencies,
            INugetAssembliesRepository nugetAssemblies)
        {
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
                        /*File.WriteAllBytes(fileName, ss);
                        var xml = File.ReadAllText(fileName);
                        string pattern = @"xmlns=""[a-zA-Z0-9:\/._]{1,}""";
                        Match m = Regex.Match(xml, pattern);
                        if (m.Success)
                            xml = xml.Replace(m.Value, "");
                        //xml = xml.Replace("xmlns=\"http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd\"", "");
                        File.WriteAllText(fileName, xml);
                        var serializer = new XmlSerializer(typeof(PackageXml));

                        using (var reader = XmlReader.Create(fileName))
                        {
                            return (PackageXml)serializer.Deserialize(reader);
                        }*/
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
                Sha = CalculateSha512(content),
                ShaAlgorithm = "SHA512",
                Size = content.Length,
            };
            data.Timestamp = commitTimestamp;

            InsertQuery(data);
            InsertRegistration(data);
            InsertPackages(data);
            InsertPackagesStorage(data, content);
            InsertDependencies(data);
            InsertAssemblies(data);
        }

        public void InsertQuery(InsertData data)
        {
            throw new NotImplementedException();
        }

        public void InsertRegistration(InsertData data)
        {
            throw new NotImplementedException();
        }

        public void InsertPackages(InsertData data)
        {
            throw new NotImplementedException();
        }

        public void InsertPackagesStorage(InsertData data, byte[] content)
        {
            throw new NotImplementedException();
        }

        public void InsertDependencies(InsertData data)
        {
            throw new NotImplementedException();
        }

        public void InsertAssemblies(InsertData data)
        {
            throw new NotImplementedException();
        }
    }
}
