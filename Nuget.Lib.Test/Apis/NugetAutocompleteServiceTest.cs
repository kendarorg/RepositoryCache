using Ioc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MultiRepositories.Repositories;
using Newtonsoft.Json;
using Nuget.Apis;
using Nuget.Lib.Test.Utils;
using Nuget.Repositories;
using NugetProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Lib.Test
{
    [TestClass]
    public class NugetAutocompleteServiceTest
    {
        private IQueryRepository _queryRepository;
        private IRepositoryEntitiesRepository _repositoryEntitiesRepository;

        private IServicesMapper _servicesMapper;
        private Guid _repoId;
        private Mock<IPackagesRepository> _packageRepositoryMock;
        private IPackagesRepository _packageRepository;
        private Mock<IQueryRepository> _queryRepositoryMock;

        [TestInitialize]
        public void Initialize()
        {

            _repoId = Guid.NewGuid();
            _packageRepositoryMock = new Mock<IPackagesRepository>();
            _packageRepository = _packageRepositoryMock.Object;
            _queryRepositoryMock = new Mock<IQueryRepository>();
            _queryRepository = _queryRepositoryMock.Object;
            var repositoryEntitiesRepository = new Mock<IRepositoryEntitiesRepository>();
            repositoryEntitiesRepository.Setup(r => r.GetByType(It.IsAny<string>())).
                Returns(new List<RepositoryEntity>
                {
                    new RepositoryEntity
                    {
                        Address = "nuget.org",
                        Id = _repoId,
                        Mirror = true,
                        Prefix = "nuget.org",
                        Settings = "",
                        Type = "nuget"
                    }
                });
            _repositoryEntitiesRepository = repositoryEntitiesRepository.Object;

            _servicesMapper = new ServicesMapperMock("nuget.org", _repoId, 5, 10, 3);
        }

        [TestMethod]
        public void ISPTogetEmptyResultAuto()
        {
            var au = new AssemblyUtils();
            var target = new NugetAutocompleteService(_queryRepository, _servicesMapper, _packageRepository);

            var result = target.Query(_repoId, new QueryModel());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Data.Count);
        }

        [TestMethod]
        public void ISPToGetReleasesAuto()
        {
            var au = new AssemblyUtils();
            var target = new NugetAutocompleteService(_queryRepository, _servicesMapper, _packageRepository);
            _queryRepositoryMock.Setup(a => a.Query(It.IsAny<Guid>(), It.IsAny<QueryModel>())).
                Returns(new List<QueryEntity>
                {
                    new QueryEntity //Prerelase
                    {
                        Version="1",
                        CsvVersions="1,2",
                        PackageId="test",
                        HasRelease=true
                    }
                });

            var result = target.Query(_repoId, new QueryModel());

            Assert.IsNotNull(result);
            JsonComp.Equals("ISPToGetReleasesAuto.json", result);
        }

        [TestMethod]
        public void ISPToGetReleasesWithPrePresentAuto()
        {
            var au = new AssemblyUtils();
            var target = new NugetAutocompleteService(_queryRepository, _servicesMapper, _packageRepository);
            _queryRepositoryMock.Setup(a => a.Query(It.IsAny<Guid>(), It.IsAny<QueryModel>())).
                Returns(new List<QueryEntity>
                {
                    new QueryEntity //Prerelase
                    {
                        Version="1",
                        CsvVersions="1,2",
                        HasRelease=true,
                        PreVersion="2",
                        PreCsvVersions="3,4",
                        PackageId="test",
                        HasPreRelease=true
                    }
                });

            var result = target.Query(_repoId, new QueryModel());

            Assert.IsNotNull(result);
            JsonComp.Equals("ISPToGetReleasesAuto.json", result);
        }

        [TestMethod]
        public void ISPToGetPreReleasesAuto()
        {
            var au = new AssemblyUtils();
            var target = new NugetAutocompleteService(_queryRepository, _servicesMapper, _packageRepository);
            _queryRepositoryMock.Setup(a => a.Query(It.IsAny<Guid>(), It.IsAny<QueryModel>())).
                Returns(new List<QueryEntity>
                {
                    new QueryEntity //Prerelase
                    {
                        PreVersion="1",
                        PreCsvVersions="1,2",
                        PackageId="test",
                        HasPreRelease=true
                    }
                });

            var result = target.Query(_repoId, new QueryModel()
            {
                PreRelease = true
            });

            Assert.IsNotNull(result);
            JsonComp.Equals("ISPToGetReleasesAuto.json", result);
        }



        [TestMethod]
        public void ISPToGetTheReleaseWhenPreReleasesIsMissingAuto()
        {
            var au = new AssemblyUtils();
            var target = new NugetAutocompleteService(_queryRepository, _servicesMapper, _packageRepository);
            _queryRepositoryMock.Setup(a => a.Query(It.IsAny<Guid>(), It.IsAny<QueryModel>())).
                Returns(new List<QueryEntity>
                {
                    new QueryEntity
                    {
                        Version="1",
                        CsvVersions="1,2",
                        PackageId="test",
                        HasRelease=true
                    }
                });

            var result = target.Query(_repoId, new QueryModel()
            {
                PreRelease = true
            });

            Assert.IsNotNull(result);
            JsonComp.Equals("ISPToGetReleasesAuto.json", result);
        }



        [TestMethod]
        public void ISPToGetMajorWhenGreaterThanPreAskingForPreAuto()
        {
            var au = new AssemblyUtils();
            var target = new NugetAutocompleteService(_queryRepository, _servicesMapper, _packageRepository);
            _queryRepositoryMock.Setup(a => a.Query(It.IsAny<Guid>(), It.IsAny<QueryModel>())).
                Returns(new List<QueryEntity>
                {
                    new QueryEntity //Prerelase
                    {
                        Version="5",
                        CsvVersions="5,6",
                        HasRelease=true,
                        PreVersion="2",
                        PreCsvVersions="3,4",
                        PackageId="test",
                        HasPreRelease=true
                    }
                });

            var result = target.Query(_repoId, new QueryModel()
            {
                PreRelease = true
            });

            Assert.IsNotNull(result);
            JsonComp.Equals("ISPToGetMajorWhenGreaterThanPreAskingForPreAuto.json", result);
        }


        [TestMethod]
        public void ISPToGetPreWhenGreaterThanMajorAskingForPreAuto()
        {
            var au = new AssemblyUtils();
            var target = new NugetAutocompleteService(_queryRepository, _servicesMapper, _packageRepository);
            _queryRepositoryMock.Setup(a => a.Query(It.IsAny<Guid>(), It.IsAny<QueryModel>())).
                Returns(new List<QueryEntity>
                {
                    new QueryEntity //Prerelase
                    {
                        Version="4",
                        CsvVersions="3.4",
                        HasRelease=true,
                        PreVersion="5",
                        PreCsvVersions="5,6",
                        PackageId="test",
                        HasPreRelease=true
                    }
                });

            var result = target.Query(_repoId, new QueryModel()
            {
                PreRelease = true
            });

            Assert.IsNotNull(result);
            JsonComp.Equals("ISPToGetMajorWhenGreaterThanPreAskingForPreAuto.json", result);
        }


        [TestMethod]
        public void ISPToGetPreReleasesVersionsToo()
        {
            var au = new AssemblyUtils();
            var target = new NugetAutocompleteService(_queryRepository, _servicesMapper, _packageRepository);
            _packageRepositoryMock.Setup(a => a.GetByPackageId(It.IsAny<Guid>(),
                It.Is<string>(x => x == "test")
                )).
                Returns(new List<PackageEntity>
                {
                    new PackageEntity
                    {
                        Version="1.0.0-pre"
                    },
                    new PackageEntity
                    {
                        Version="1.0.0"
                    }
                });

            var result = target.QueryByPackage(_repoId, "test", true);

            Assert.IsNotNull(result);
            JsonComp.Equals("ISPToGetPreReleasesVersionsToo.json", result);
        }


        [TestMethod]
        public void ISPToGetReleasesVersionsOnly()
        {
            var au = new AssemblyUtils();
            var target = new NugetAutocompleteService(_queryRepository, _servicesMapper, _packageRepository);
            _packageRepositoryMock.Setup(a => a.GetByPackageId(It.IsAny<Guid>(),
                It.Is<string>(x => x == "test")
                )).
                Returns(new List<PackageEntity>
                {
                    new PackageEntity
                    {
                        Version="1.0.0-pre"
                    },
                    new PackageEntity
                    {
                        Version="1.0.0"
                    }
                });

            var result = target.QueryByPackage(_repoId, "test", false);

            Assert.IsNotNull(result);
            JsonComp.Equals("ISPToGetReleasesVersionsOnly.json", result);
        }
    }

    /*new QueryEntity
                    {
                        Version="",
                        PreVersion="",
                        CsvVersions="",
                        CsvPreVersions="",
                        PackageId="",
                        HasRelease=true,
                        HasPreRelease=true
                    }*/
}
