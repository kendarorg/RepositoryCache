using Ioc;
using NUnit.Framework;
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
    [TestFixture]
    public class NugetRegistrationServiceTest
    {
        private IRegistrationRepository _registrationRepository;
        private Mock<IRegistrationRepository> _registrationRepositoryMock;
        private ICatalogService _catalogService;
        private Mock<ICatalogService> _catalogServiceMock;

        private IServicesMapper _servicesMapper;
        private Guid _repoId;

        [SetUp]
        public void Initialize()
        {
            _repoId = Guid.NewGuid();
            _servicesMapper = new ServicesMapperMock("nuget.org", _repoId, 0, 5, 3);

            _registrationRepositoryMock = new Mock<IRegistrationRepository>();
            _registrationRepository = _registrationRepositoryMock.Object;

            _catalogServiceMock = new Mock<ICatalogService>();
            _catalogService = _catalogServiceMock.Object;

            _servicesMapper = new ServicesMapperMock("nuget.org", _repoId, 5, 10, 3);
        }


        private static List<PackageDetail> GetOnePageCatalogResult()
        {
            return new List<PackageDetail>
            {
                new PackageDetail("test/1.0.0alpha","pd","registration","test","1.0.0-alpha","content"),
                new PackageDetail("test/1.1.0","pd","registration","test","1.1.0","content"),
                new PackageDetail("test/1.2.0","pd","registration","test","1.2.0","content")
            };
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



        private IEnumerable<RegistrationEntity> GetMultiPageRegistrationResult(int maxRegPages, DateTime time, DateTime lastTime, Guid lastCommit)
        {
            var firstPage = new List<RegistrationEntity>();
            for (int i = 0; i < maxRegPages; i++)
            {
                yield return new RegistrationEntity
                {
                    CommitId = Guid.Parse("ced6c9a6-860f-4eb0-83b2-9b84cdeb0eb1"),
                    CommitTimestamp = time,
                    PackageId = "test",
                    Version = "0.1." + i,
                    Major = 0,
                    Minor = 1,
                    Patch = i
                };
            }
            yield return new RegistrationEntity
            {
                CommitId = Guid.Parse("ced6c9a6-860f-4eb0-83b2-9b84cdeb0eb1"),
                CommitTimestamp = time,
                PackageId = "test",
                Version = "1.0.0-alpha",
                Major = 1,
                Minor = 0,
                Patch = 0,
                PreRelease = "alpha"
            };
            yield return new RegistrationEntity
            {
                CommitId = Guid.Parse("ced6c9a6-860f-4eb0-83b2-9b84cdeb0eb2"),
                CommitTimestamp = time,
                PackageId = "test",
                Version = "1.1.0",
                Major = 1,
                Minor = 1,
                Patch = 0
            };
            yield return new RegistrationEntity
            {
                CommitId = lastCommit,
                CommitTimestamp = lastTime,
                PackageId = "test",
                Version = "1.2.0",
                Major = 1,
                Minor = 2,
                Patch = 0
            };
        }



        [Test]
        public void ISPTGetLeafNoSemVer()
        {
            var target = new NugetRegistrationService(_registrationRepository, _servicesMapper, _catalogService);
            var time = new DateTime(123456789);
            _registrationRepositoryMock.Setup(a => a.GetSpecific(
                It.Is<Guid>(g => g == _repoId),
                It.Is<String>(g => g == "test"),
                It.Is<String>(g => g == "1.0.0"))).Returns(new RegistrationEntity
                {
                    CommitId = Guid.NewGuid(),
                    CommitTimestamp = time,
                    PackageId = "test",
                    Version = "1.0.0"
                });

            var result = target.Leaf(_repoId, "test", "1.0.0", "test.1.0.0", null);

            Assert.IsNotNull(result);
            JsonComp.Equals("ISPTGetLeafNoSemVer.json", result);
        }

        [Test]
        public void ISPTGetOnePageOnly()
        {

            var target = new NugetRegistrationService(_registrationRepository, _servicesMapper, _catalogService);
            var time = new DateTime(123456789);
            var lastTime = new DateTime(223456789);
            var lastCommit = Guid.Parse("0006c9a6-860f-4eb0-83b2-9b84cdeb0eb1");

            _registrationRepositoryMock.Setup(a => a.GetAllByPackageId(
                It.Is<Guid>(g => g == _repoId),
                It.Is<String>(g => g == "test"))).Returns(GetOnePageRegistrationResult(time, lastTime, lastCommit));

            _registrationRepositoryMock.Setup(a => a.GetRange(
                It.Is<Guid>(g => g == _repoId),
                It.Is<String>(g => g == "test"),
                It.Is<String>(g => g == "1.0.0-alpha"),
                It.Is<String>(g => g == "1.2.0"))).Returns(GetOnePageRegistrationResult(time, lastTime, lastCommit));

            List<PackageDetail> packDetails = GetOnePageCatalogResult();
            _catalogServiceMock.Setup(a => a.GetPackageDetailsForRegistration(
                It.Is<Guid>(g => g == _repoId),
                It.Is<String>(g => g == "test"),
                It.IsAny<string>(),
                It.Is<String>(g => g == "1.0.0-alpha"),
                It.Is<String>(g => g == "1.1.0"),
                It.Is<String>(g => g == "1.2.0"))).Returns(packDetails);


            var result = target.IndexPage(_repoId, "test", null);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual(3, result.Items[0].Count);
            Assert.AreEqual(3, result.Items[0].Items.Count);
            JsonComp.Equals("ISPTGetOnePageOnly.json", result);
        }

        [Test]
        public void ISPTGetMultipage()
        {

            var target = new NugetRegistrationService(_registrationRepository, _servicesMapper, _catalogService);
            var time = new DateTime(123456789);
            var lastTime = new DateTime(223456789);
            var lastCommit = Guid.Parse("0006c9a6-860f-4eb0-83b2-9b84cdeb0eb1");

            _registrationRepositoryMock.Setup(a => a.GetAllByPackageId(
                It.Is<Guid>(g => g == _repoId),
                It.Is<String>(g => g == "test"))).Returns(GetMultiPageRegistrationResult(
                    _servicesMapper.MaxRegistrationPages(_repoId),
                    time, lastTime, lastCommit));

            var result = target.IndexPage(_repoId, "test", null);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(3, result.Items.Count);
            JsonComp.Equals("ISPTGetMultipage.json", result);
        }


        [Test]
        public void ISPTGetSinglePage()
        {

            var target = new NugetRegistrationService(_registrationRepository, _servicesMapper, _catalogService);
            var time = new DateTime(123456789);
            var lastTime = new DateTime(223456789);
            var lastCommit = Guid.Parse("0006c9a6-860f-4eb0-83b2-9b84cdeb0eb1");

            _registrationRepositoryMock.Setup(a => a.GetAllByPackageId(
                It.Is<Guid>(g => g == _repoId),
                It.Is<String>(g => g == "test"))).Returns(GetOnePageRegistrationResult(time, lastTime, lastCommit));

            _registrationRepositoryMock.Setup(a => a.GetRange(
                It.Is<Guid>(g => g == _repoId),
                It.Is<String>(g => g == "test"),
                It.Is<String>(g => g == "1.0.0-alpha"),
                It.Is<String>(g => g == "1.2.0"))).Returns(GetOnePageRegistrationResult(time, lastTime, lastCommit));

            List<PackageDetail> packDetails = GetOnePageCatalogResult();
            _catalogServiceMock.Setup(a => a.GetPackageDetailsForRegistration(
                It.Is<Guid>(g => g == _repoId),
                It.Is<String>(g => g == "test"),
                It.IsAny<string>(),
                It.Is<String>(g => g == "1.0.0-alpha"),
                It.Is<String>(g => g == "1.1.0"),
                It.Is<String>(g => g == "1.2.0"))).Returns(packDetails);


            var result = target.SinglePage(_repoId, "test", "1.0.0-alpha", "1.2.0", null);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(3, result.Items.Count);
            JsonComp.Equals("ISPTGetSinglePage.json", result);
        }
    }
}
