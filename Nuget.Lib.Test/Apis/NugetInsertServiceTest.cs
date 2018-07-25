using Ioc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nuget.Repositories;
using Nuget.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NugetProtocol;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using Repositories;

namespace Nuget.Lib.Test.Apis
{
    [TestClass]
    public class NugetInsertServiceTest
    {
/*        private IRegistrationRepository _registrationRepository;
        private ServicesMapperMock _servicesMapper;
        private AssemblyUtils _as;
        private ITransaction _tx;
        private Mock<IRegistrationRepository> _registrationRepositoryMock;

        private Guid _repoId;
        private Mock<IPackagesStorage> _packagesStorageMock;
        private IPackagesStorage _packagesStorage;
        private Mock<IQueryRepository> _queryRepositoryMock;
        private IQueryRepository _queryRepository;
        private Mock<IPackagesRepository> _packagesRepositoryMock;
        private IPackagesRepository _packagesRepository;


        private INugetAssembliesRepository _nugetAssemblies;
        private INugetDependenciesRepository _nugetDependencies;

        private Mock<INugetAssembliesRepository> _nugetAssembliesMock;
        private Mock<INugetDependenciesRepository> _nugetDependenciesMock;

        [TestInitialize]
        public void Initialize()
        {
            _repoId = Guid.NewGuid();

            _packagesStorageMock = new Mock<IPackagesStorage>();
            _packagesStorage = _packagesStorageMock.Object;

            _queryRepositoryMock = new Mock<IQueryRepository>();
            _queryRepository = _queryRepositoryMock.Object;

            _packagesRepositoryMock = new Mock<IPackagesRepository>();
            _packagesRepository = _packagesRepositoryMock.Object;

            _registrationRepositoryMock = new Mock<IRegistrationRepository>();
            _registrationRepository = _registrationRepositoryMock.Object;

            _nugetAssembliesMock = new Mock<INugetAssembliesRepository>();
            _nugetAssemblies = _nugetAssembliesMock.Object;

            _nugetDependenciesMock = new Mock<INugetDependenciesRepository>();
            _nugetDependencies = _nugetDependenciesMock.Object;

            _servicesMapper = new ServicesMapperMock("nuget.org", _repoId, 5, 0);
            _as = new AssemblyUtils();
            _tx = new Mock<ITransaction>().Object;
        }

        private InsertData SetupData()
        {
            var packageXml = _as.ReadRes<NugetInsertServiceTest>("Complex.nuspec");

            var data = new InsertData
            {
                RepoId = Guid.Parse("2226c9a6-860f-4eb0-83b2-9b84cdeb0eb1"),
                CommitId = Guid.Parse("0006c9a6-860f-4eb0-83b2-9b84cdeb0eb1"),
                Nuspec = Deserialize(packageXml),
                HashKey = "123456==",
                HashAlgorithm = "SHA512",
                Size = 100,
                Timestamp = new DateTime(1000),
                Id= "System.Security.Principal.Windows",
                Version= "4.5.0"
            };
            return data;
        }

        private PackageXml Deserialize(string xml)
        {
            string pattern = @"xmlns=""[a-zA-Z0-9:\/._]{1,}""";
            System.Text.RegularExpressions.Match m = Regex.Match(xml, pattern);
            if (m.Success)
            {
                xml = xml.Replace(m.Value, "");
            }
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml));
            ms.Seek(0, SeekOrigin.Begin);
            var serializer = new XmlSerializer(typeof(PackageXml));

            using (var reader = XmlReader.Create(ms))
            {
                return (PackageXml)serializer.Deserialize(reader);
            }
        }

        [TestMethod]
        public void ISPToInsertQuery()
        {

            var target = new InsertNugetService(
                _queryRepository, _registrationRepository,
                _packagesRepository, _packagesStorage, _nugetDependencies, _nugetAssemblies);
            InsertData data = SetupData();

            target.InsertQuery(data, _tx);
        }

        [TestMethod]
        public void ISPToInsertRegistration()
        {

            var target = new InsertNugetService(
                _queryRepository, _registrationRepository,
                _packagesRepository, _packagesStorage, _nugetDependencies, _nugetAssemblies);
            InsertData data = SetupData();

            target.InsertRegistration(data_tx);
        }

        [TestMethod]
        public void ISPToInsertPackages()
        {

            var target = new InsertNugetService(
                _queryRepository, _registrationRepository,
                _packagesRepository, _packagesStorage, _nugetDependencies, _nugetAssemblies);
            InsertData data = SetupData();

            target.InsertPackages(data, _tx);
        }

        [TestMethod]
        public void ISPToInsertStorage()
        {

            var target = new InsertNugetService(
                _queryRepository, _registrationRepository,
                _packagesRepository, _packagesStorage, _nugetDependencies, _nugetAssemblies);
            InsertData data = SetupData();

            target.InsertPackagesStorage(data, new byte[] { });
        }

        [TestMethod]
        public void ISPToInsertDeps()
        {

            var target = new InsertNugetService(
                _queryRepository, _registrationRepository,
                _packagesRepository, _packagesStorage, _nugetDependencies, _nugetAssemblies);
            InsertData data = SetupData();

            target.InsertDependencies(data, _tx);
        }

        [TestMethod]
        public void ISPToInsertAsms()
        {

            var target = new InsertNugetService(
                _queryRepository, _registrationRepository,
                _packagesRepository, _packagesStorage, _nugetDependencies, _nugetAssemblies);
            InsertData data = SetupData();

            target.InsertAssemblies(data, _tx);
        }*/
    }
}
