using Ioc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MultiRepositories.Repositories;
using Newtonsoft.Json;
using Nuget.Apis;
using Nuget.Lib.Test.Utils;
using Nuget.Repositories;
using Nuget.Services;
using NugetProtocol;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Lib.Test
{
    [TestClass]
    public class NugetBaseAddressServiceTest
    {
        private IRegistrationRepository _registrationRepository;
        private IRepositoryEntitiesRepository _repositoryEntitiesRepository;
        private Mock<IPackagesStorage> _packagesStorageMock;
        private IPackagesStorage _packagesStorage;
        private Mock<IRegistrationRepository> _registrationRepositoryMock;

        private Guid _repoId;
        private Mock<IPackagesRepository> _packagesRepositoryMock;
        private IPackagesRepository _packagesRepository;

        [TestInitialize]
        public void Initialize()
        {
            _repoId = Guid.NewGuid();

            _packagesRepositoryMock = new Mock<IPackagesRepository>();
            _packagesRepository = _packagesRepositoryMock.Object;

            _registrationRepositoryMock = new Mock<IRegistrationRepository>();
            _registrationRepository = _registrationRepositoryMock.Object;

            var repositoryEntitiesRepository = new Mock<IRepositoryEntitiesRepository>();

            var repo = new RepositoryEntity
            {
                Address = "nuget.org",
                Id = _repoId,
                Mirror = true,
                Prefix = "nuget.org",
                Settings = "",
                Type = "nuget",
                PackagesPath = "path"
            };
            /*repositoryEntitiesRepository.Setup(r => r.GetByType(It.IsAny<string>())).
                Returns(new List<RepositoryEntity>
                {
                    repo
                });*/
            repositoryEntitiesRepository.Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<ITransaction>())).
                Returns(repo);
            _repositoryEntitiesRepository = repositoryEntitiesRepository.Object;

            _packagesStorageMock = new Mock<IPackagesStorage>();
            _packagesStorage = _packagesStorageMock.Object;
        }


        private static List<RegistrationEntity> GetOnePageRegistrationResult(DateTime time, DateTime lastTime, Guid lastCommit)
        {
            return new List<RegistrationEntity>()
                {
                    new RegistrationEntity
                    {
                        CommitId = Guid.Parse("ced6c9a6-860f-4eb0-83b2-9b84cdeb0eb1"),
                        CommitTimestamp = time,
                        PackageId = "test",
                        Version = "1.0.0-alpha",
                        Major=1,
                        Minor=0,
                        Patch=0,
                        PreRelease="alpha"
                    },
                    new RegistrationEntity
                    {
                        CommitId = Guid.Parse("ced6c9a6-860f-4eb0-83b2-9b84cdeb0eb2"),
                        CommitTimestamp = time,
                        PackageId = "test",
                        Version = "1.1.0",
                        Major=1,
                        Minor=1,
                        Patch=0
                    },
                    new RegistrationEntity
                    {
                        CommitId =  lastCommit,
                        CommitTimestamp = lastTime,
                        PackageId = "test",
                        Version = "1.2.0",
                        Major=1,
                        Minor=2,
                        Patch=0
                    }
                };
        }



        [TestMethod]
        public void ISPToGetNugetPackage()
        {

            var target = new NugetBaseAddressService(
                _packagesRepository,
                _registrationRepository,
                _repositoryEntitiesRepository,
                _packagesStorage);

            var time = new DateTime(123456789);
            var lastTime = new DateTime(223456789);
            var lastCommit = Guid.Parse("0006c9a6-860f-4eb0-83b2-9b84cdeb0eb1");
            _packagesRepositoryMock.Setup(r => r.GetByIdVersion(
                It.Is<Guid>(a => a == _repoId),
                It.Is<string>(a => a == "test.1.0.0"))).
                Returns(new PackageEntity()
                {
                    Version = "1.0.0",
                    PackageId = "test"
                });
            _packagesStorageMock.Setup(a => a.Load(
                It.IsAny<RepositoryEntity>(),
                It.Is<string>(k => k == "test"),
                It.Is<string>(k => k == "1.0.0")
                )).Returns(new byte[] { 0, 0 });

            var result = target.GetNupkg(_repoId, "test.1.0.0");
            Assert.AreEqual(2, result.Length);
        }


        [TestMethod]
        public void ISPToGetNugetVersion()
        {

            var target = new NugetBaseAddressService(
                _packagesRepository,
                _registrationRepository,
                _repositoryEntitiesRepository,
                _packagesStorage);

            var time = new DateTime(123456789);
            var lastTime = new DateTime(223456789);
            var lastCommit = Guid.Parse("0006c9a6-860f-4eb0-83b2-9b84cdeb0eb1");
            _registrationRepositoryMock.Setup(a => a.GetAllByPackageId(
                It.Is<Guid>(g => g == _repoId),
                It.Is<String>(g => g == "test"))).Returns(GetOnePageRegistrationResult(time, lastTime, lastCommit));

            var result = target.GetVersions(_repoId, "test");
            Assert.AreEqual(3, result.Versions.Count);
            JsonComp.Equals("ISPToGetNugetVersion.json", result);
        }
    }
}
