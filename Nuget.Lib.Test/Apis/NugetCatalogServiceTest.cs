using Ioc;
using NUnit.Framework;
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
    [TestFixture]
    public class NugetCatalogServiceTest
    {
        private IRegistrationRepository _registrationRepository;
        private ServicesMapperMock _servicesMapper;
        private Mock<IRegistrationRepository> _registrationRepositoryMock;

        private Guid _repoId;
        private Mock<IPackagesRepository> _packagesRepositoryMock;
        private IPackagesRepository _packagesRepository;


        private INugetAssembliesRepository _nugetAssemblies;
        private INugetDependenciesRepository _nugetDependencies;

        private Mock<INugetAssembliesRepository> _nugetAssembliesMock;
        private Mock<INugetDependenciesRepository> _nugetDependenciesMock;
        [SetUp]
        public void Initialize()
        {
            _repoId = Guid.NewGuid();

            _packagesRepositoryMock = new Mock<IPackagesRepository>();
            _packagesRepository = _packagesRepositoryMock.Object;

            _registrationRepositoryMock = new Mock<IRegistrationRepository>();
            _registrationRepository = _registrationRepositoryMock.Object;

            _nugetAssembliesMock = new Mock<INugetAssembliesRepository>();
            _nugetAssemblies = _nugetAssembliesMock.Object;

            _nugetDependenciesMock = new Mock<INugetDependenciesRepository>();
            _nugetDependencies = _nugetDependenciesMock.Object;

            _servicesMapper = new ServicesMapperMock("nuget.org", _repoId, 5, 0,3);
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



        [Test]
        public void ISPToGetCatalog()
        {

            var target = new NugetCatalogService(
                _registrationRepository,
                _servicesMapper,
                _packagesRepository,
                _nugetDependencies,
                _nugetAssemblies
                );

            var resultList = new List<RegistrationEntity>();
            for (int i = 0; i < 7; i++)
            {
                resultList.Add(new RegistrationEntity
                {
                    CommitId = Guid.Parse("ced6c9a6-860f-4eb0-83b2-9b84cdeb0eb" + i),
                    CommitTimestamp = new DateTime(100000 + i)
                });
            }
            _registrationRepositoryMock.Setup(a => a.GetAllOrderByDate(It.IsAny<Guid>())).
                Returns(resultList);

            var result = target.GetCatalog(_repoId);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Items.Count);
            JsonComp.Equals("ISPToGetCatalog.json", result);
        }

        [Test]
        public void ISPToGetCatalogPage()
        {

            var target = new NugetCatalogService(
                _registrationRepository,
                _servicesMapper,
                _packagesRepository,
                _nugetDependencies,
                _nugetAssemblies);

            var resultList = new List<RegistrationEntity>();
            for (int i = 0; i < 5; i++)
            {
                resultList.Add(new RegistrationEntity
                {
                    CommitId = Guid.Parse("ced6c9a6-860f-4eb0-83b2-9b84cdeb0e1" + i),
                    CommitTimestamp = new DateTime(100000 + i),
                    PackageId = "test" + i,
                    Version = 1 + ".0.0"
                });
            }
            _registrationRepositoryMock.Setup(a => a.GetPage(It.IsAny<Guid>(),
                It.Is<int>(x => x == 1 * _servicesMapper.MaxCatalogPages(_repoId)),
                It.Is<int>(x => x == _servicesMapper.MaxCatalogPages(_repoId))
                )).
                Returns(resultList);

            var result = target.GetCatalogPage(_repoId, 1);
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Items.Count);
            JsonComp.Equals("ISPToGetCatalogPage.json", result);
        }

        [Test]
        public void ISPToGetSimplePackageCatalog()
        {

            var target = new NugetCatalogService(
                _registrationRepository,
                _servicesMapper,
                _packagesRepository,
                _nugetDependencies,
                _nugetAssemblies);

            var package = new PackageEntity()
            {
                CommitTimestamp = new DateTime(1998, 12, 31, 12, 12, 12),
                PackageId = "test",
                Version = "1.0.0",
                PackageIdAndVersion = "test.1.0.0",
                CommitId = Guid.Parse("ced6c9a6-860f-4eb0-83b2-9b84cdeb0e10"),
                HashAlgorithm = "SHA256",
                HashKey = "123456===",
                Size = 1000,

            };
            _packagesRepositoryMock.Setup(a => a.GetByIdVersion(It.IsAny<Guid>(),
                It.Is<string>(x => x == "test.1.0.0")
                )).
                Returns(package);

            var result = target.GetPackageCatalog(_repoId, "1998.12.31.12.12.12", "test.1.0.0");
            Assert.IsNotNull(result);
            JsonComp.Equals("ISPToGetSimplePackageCatalog.json", result);
        }


        [Test]
        public void ISPToGetPackageDepsCatalog()
        {

            var target = new NugetCatalogService(
                _registrationRepository,
                _servicesMapper,
                _packagesRepository,
                _nugetDependencies,
                _nugetAssemblies);

            var package = new PackageEntity()
            {
                CommitTimestamp = new DateTime(1998, 12, 31, 12, 12, 12),
                PackageId = "test",
                Version = "1.0.0",
                PackageIdAndVersion = "test.1.0.0",
                CommitId = Guid.Parse("ced6c9a6-860f-4eb0-83b2-9b84cdeb0e10"),
                HashAlgorithm = "SHA256",
                HashKey = "123456===",
                Size = 1000,

            };
            _packagesRepositoryMock.Setup(a => a.GetByIdVersion(It.IsAny<Guid>(),
                It.Is<string>(x => x == "test.1.0.0")
                )).
                Returns(package);

            _nugetDependenciesMock.Setup(a => a.GetDependencies(It.IsAny<Guid>(),
                It.Is<string>(x => x == "test"),
                It.Is<string>(x => x == "1.0.0")
                )).
                Returns(new List<NugetDependency>
                {
                    new NugetDependency
                    {
                        PackageId="sub.dep",
                        Range ="2.0.0",
                        TargetFramework= ".net45"
                    },
                    new NugetDependency
                    {
                        PackageId="sub.dep",
                        Range ="3.0.0",
                        TargetFramework= ".net31"
                    }
                });

            var result = target.GetPackageCatalog(_repoId, "1998.12.31.12.12.12", "test.1.0.0");
            Assert.IsNotNull(result);
            JsonComp.Equals("ISPToGetPackageDepsCatalog.json", result);
        }

        //https://api.nuget.org/v3/catalog0/data/2018.05.29.20.29.29/system.security.principal.windows.4.5.0.json

        [Test]
        public void ISPToGetPackageFwDepsCatalog()
        {

            var target = new NugetCatalogService(
                _registrationRepository,
                _servicesMapper,
                _packagesRepository,
                _nugetDependencies,
                _nugetAssemblies);

            var package = new PackageEntity()
            {
                CommitTimestamp = new DateTime(1998, 12, 31, 12, 12, 12),
                PackageId = "test",
                Version = "1.0.0",
                PackageIdAndVersion = "test.1.0.0",
                CommitId = Guid.Parse("ced6c9a6-860f-4eb0-83b2-9b84cdeb0e10"),
                HashAlgorithm = "SHA256",
                HashKey = "123456===",
                Size = 1000,

            };
            _packagesRepositoryMock.Setup(a => a.GetByIdVersion(It.IsAny<Guid>(),
                It.Is<string>(x => x == "test.1.0.0")
                )).
                Returns(package);

            _nugetAssembliesMock.Setup(a => a.GetGroups(It.IsAny<Guid>(),
                It.Is<string>(x => x == "test"),
                It.Is<string>(x => x == "1.0.0")
                )).
                Returns(new List<NugetAssemblyGroup>
                {
                    new NugetAssemblyGroup
                    {
                        AssemblyName="System.Web",
                        TargetFramework=".net35",
                    },
                    new NugetAssemblyGroup
                    {
                        AssemblyName="System.Admin",
                        TargetFramework=".net40",
                    }
                });

            var result = target.GetPackageCatalog(_repoId, "1998.12.31.12.12.12", "test.1.0.0");
            Assert.IsNotNull(result);
            JsonComp.Equals("ISPToGetPackageFwDepsCatalog.json", result);
        }


        [Test]
        public void ISPToGetPackageEmptyDepsCatalog()
        {

            var target = new NugetCatalogService(
                _registrationRepository,
                _servicesMapper,
                _packagesRepository,
                _nugetDependencies,
                _nugetAssemblies);

            var package = new PackageEntity()
            {
                CommitTimestamp = new DateTime(1998, 12, 31, 12, 12, 12),
                PackageId = "test",
                Version = "1.0.0",
                PackageIdAndVersion = "test.1.0.0",
                CommitId = Guid.Parse("ced6c9a6-860f-4eb0-83b2-9b84cdeb0e10"),
                HashAlgorithm = "SHA256",
                HashKey = "123456===",
                Size = 1000,

            };
            _packagesRepositoryMock.Setup(a => a.GetByIdVersion(It.IsAny<Guid>(),
                It.Is<string>(x => x == "test.1.0.0")
                )).
                Returns(package);

            _nugetDependenciesMock.Setup(a => a.GetDependencies(It.IsAny<Guid>(),
                It.Is<string>(x => x == "test"),
                It.Is<string>(x => x == "1.0.0")
                )).
                Returns(new List<NugetDependency>
                {
                    new NugetDependency
                    {
                        TargetFramework= ".net45"
                    },
                    new NugetDependency
                    {
                        PackageId="sub.dep",
                        Range ="3.0.0",
                        TargetFramework= ".net31"
                    }
                });

            var result = target.GetPackageCatalog(_repoId, "1998.12.31.12.12.12", "test.1.0.0");
            Assert.IsNotNull(result);
            JsonComp.Equals("ISPToGetPackageEmptyDepsCatalog.json", result);
        }


        [Test]
        public void ISPToGetPackageListCatalogWithDeps()
        {

            var target = new NugetCatalogService(
                _registrationRepository,
                _servicesMapper,
                _packagesRepository,
                _nugetDependencies,
                _nugetAssemblies);

            var package = new PackageEntity()
            {
                CommitTimestamp = new DateTime(1998, 12, 31, 12, 12, 12),
                PackageId = "test",
                Version = "1.0.0",
                PackageIdAndVersion = "test.1.0.0",
                CommitId = Guid.Parse("ced6c9a6-860f-4eb0-83b2-9b84cdeb0e10"),
                HashAlgorithm = "SHA256",
                HashKey = "123456===",
                Size = 1000,

            };
            _packagesRepositoryMock.Setup(a => a.GetByIdVersions(It.IsAny<Guid>(),
                It.Is<string>(x => x == "test"),
                It.Is<string[]>(x => x[0] == "1.0.0")
                )).
                Returns(new List<PackageEntity>
                {
                    package
                });

            _nugetDependenciesMock.Setup(a => a.GetDependencies(It.IsAny<Guid>(),
                It.Is<string>(x => x == "test"),
                It.Is<string>(x => x == "1.0.0")
                )).
                Returns(new List<NugetDependency>
                {
                    new NugetDependency
                    {
                        TargetFramework= ".net45"
                    },
                    new NugetDependency
                    {
                        PackageId="sub.dep",
                        Range ="3.0.0",
                        TargetFramework= ".net31"
                    }
                });

            var result = target.GetPackageDetailsForRegistration(_repoId, "test", null, "1.0.0");
            Assert.IsNotNull(result);
            JsonComp.Equals("ISPToGetPackageListCatalogWithDeps.json", result);
        }
    }
}
