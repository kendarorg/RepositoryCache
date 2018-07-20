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
    public class NugetRegistrationServiceTest
    {
        private IRegistrationRepository _registrationRepository;
        private Mock<IRegistrationRepository> _registrationRepositoryMock;
        private IRegistrationPageRepository _registrationPageRepository;
        private Mock<IRegistrationPageRepository> _registrationPageRepositoryMock;
        private ICatalogService _catalogService;
        private Mock<ICatalogService> _catalogServiceMock;

        private IServicesMapper _servicesMapper;
        private Guid _repoId;

        [TestInitialize]
        public void Initialize()
        {
            _repoId = Guid.NewGuid();
            _servicesMapper = new ServicesMapperMock("nuget.org", _repoId);

            _registrationRepositoryMock = new Mock<IRegistrationRepository>();
            _registrationRepository = _registrationRepositoryMock.Object;

            _registrationPageRepositoryMock = new Mock<IRegistrationPageRepository>();
            _registrationPageRepository = _registrationPageRepositoryMock.Object;

            _catalogServiceMock = new Mock<ICatalogService>();
            _catalogService = _catalogServiceMock.Object;
            /*_repoId = Guid.NewGuid();
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
            */
            _servicesMapper = new ServicesMapperMock("nuget.org", _repoId);
        }

        [TestMethod]
        public void ISPTGetLeafNoSemVer()
        {
            var target = new NugetRegistrationService(_registrationRepository, _registrationPageRepository, _servicesMapper, _catalogService);
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
    }
}
