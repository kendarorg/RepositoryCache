using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MultiRepositories;
using MultiRepositories.Repositories;
using Nuget.Controllers;
using NugetProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Lib.Test.Controllers
{
    [TestClass]
    public class V3Beat_QueryTest
    {
        private Guid _repoId;
        private Mock<ISearchQueryService> _nugetServiceMock;
        private ISearchQueryService _nugetService;
        private AppProperties _properties;
        private Mock<IRepositoryEntitiesRepository> _repsMock;
        private IRepositoryEntitiesRepository _reps;
        private ServicesMapperMock _servicesMapper;

        [TestInitialize]
        public void Initialize()
        {
            _repoId = Guid.NewGuid();
            _nugetServiceMock = new Mock<ISearchQueryService>();
            _nugetService = _nugetServiceMock.Object;
            _properties = new AppProperties("localhost", "");
            _repsMock = new Mock<IRepositoryEntitiesRepository>();
            _reps = _repsMock.Object;
            _servicesMapper = new ServicesMapperMock("nuget.org", _repoId);
        }

        [TestMethod]
        public void ISPToQuery()
        {
            var target = new V3beta_Query("test", _nugetService, _properties, _reps, _servicesMapper);

            var serializableRequest = new SerializableRequest
            {

            };
        }
    }
}
