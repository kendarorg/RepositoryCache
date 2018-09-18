using Ioc;
using NUnit.Framework;
using Moq;
using MultiRepositories.Repositories;
using Newtonsoft.Json;
using Nuget.Apis;
using Nuget.Lib.Test.Utils;
using Nuget.Repositories;
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
    public class NugetSearchQueryServiceTest
    {
        private IQueryRepository _queryRepository;
        private IRepositoryEntitiesRepository _repositoryEntitiesRepository;

        private IServicesMapper _servicesMapper;
        private Guid _repoId;
        private Mock<IQueryRepository> _queryRepositoryMock;

        [SetUp]
        public void Initialize()
        {

            _repoId = Guid.NewGuid();
            _queryRepositoryMock = new Mock<IQueryRepository>();
            _queryRepository = _queryRepositoryMock.Object;
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
            repositoryEntitiesRepository.Setup(r => r.GetById(It.IsAny<Guid>(), It.IsAny<ITransaction>())).
                Returns(repo);

            repositoryEntitiesRepository.Setup(r => r.GetByType(It.IsAny<string>())).
                Returns(new List<RepositoryEntity>
                {
                    repo
                });
            _repositoryEntitiesRepository = repositoryEntitiesRepository.Object;

            _servicesMapper = new ServicesMapperMock("nuget.org", _repoId, 5, 10, 3);
        }

        [Test]
        public void ISPTogetEmptyResult()
        {
            var au = new AssemblyUtils();
            var target = new NugetSearchQueryService(_queryRepository, _repositoryEntitiesRepository, _servicesMapper);

            var result = target.Query(_repoId, new QueryModel());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Data.Count);
        }

        [Test]
        public void ISPToGetReleases()
        {
            var au = new AssemblyUtils();
            var target = new NugetSearchQueryService(_queryRepository, _repositoryEntitiesRepository, _servicesMapper);
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
            JsonComp.Equals("ISPToGetReleases.json", result);
        }

        [Test]
        public void ISPToGetReleasesWithPrePresent()
        {
            var au = new AssemblyUtils();
            var target = new NugetSearchQueryService(_queryRepository, _repositoryEntitiesRepository, _servicesMapper);
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
            JsonComp.Equals("ISPToGetReleases.json", result);
        }

        [Test]
        public void ISPToGetPreReleases()
        {
            var au = new AssemblyUtils();
            var target = new NugetSearchQueryService(_queryRepository, _repositoryEntitiesRepository, _servicesMapper);
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
            JsonComp.Equals("ISPToGetReleases.json", result);
        }



        [Test]
        public void ISPToGetTheReleaseWhenPreReleasesIsMissing()
        {
            var au = new AssemblyUtils();
            var target = new NugetSearchQueryService(_queryRepository, _repositoryEntitiesRepository, _servicesMapper);
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
            JsonComp.Equals("ISPToGetReleases.json", result);
        }



        [Test]
        public void ISPToGetMajorWhenGreaterThanPreAskingForPre()
        {
            var au = new AssemblyUtils();
            var target = new NugetSearchQueryService(_queryRepository, _repositoryEntitiesRepository, _servicesMapper);
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
            JsonComp.Equals("ISPToGetMajorWhenGreaterThanPreAskingForPre.json", result);
        }


        [Test]
        public void ISPToGetPreWhenGreaterThanMajorAskingForPre()
        {
            var au = new AssemblyUtils();
            var target = new NugetSearchQueryService(_queryRepository, _repositoryEntitiesRepository, _servicesMapper);
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
            JsonComp.Equals("ISPToGetMajorWhenGreaterThanPreAskingForPre.json", result);
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
