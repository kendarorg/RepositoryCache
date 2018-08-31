using Ioc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MultiRepositories;
using MultiRepositories.Repositories;
using Newtonsoft.Json;
using Nuget.Controllers;
using Nuget.Lib.Test.Utils;
using NugetProtocol;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuget.Lib.Test.Controllers
{
    [TestClass]
    public class V3Beat_QueryTest : BaseControllersTest
    {
        private Guid _repoId;
        private Mock<ISearchQueryService> _nugetServiceMock;
        private ISearchQueryService _nugetService;
        private AppProperties _properties;
        private Mock<IRepositoryEntitiesRepository> _repsMock;
        private IRepositoryEntitiesRepository _reps;
        private IServicesMapper _servicesMapper;
        private AssemblyUtils _assemblyUtils;

        [TestInitialize]
        public void Initialize()
        {
            _assemblyUtils = new AssemblyUtils();
            var data = _assemblyUtils.ReadRes<NugetServicesMapperTest>("nuget.org.settings.json");
            _repoId = Guid.NewGuid();
            _nugetServiceMock = new Mock<ISearchQueryService>();
            _nugetService = _nugetServiceMock.Object;
            _properties = new AppProperties(null, null);
            _repsMock = new Mock<IRepositoryEntitiesRepository>();
            var repo = new RepositoryEntity
            {
                Address = "nuget.org",
                Id = _repoId,
                Mirror = true,
                Prefix = "nuget.org",
                Settings = data,
                Type = "nuget"
            };
            _repsMock.Setup(r => r.GetByType(It.IsAny<string>())).
                Returns(new List<RepositoryEntity>
                {
                    repo
                });
            _repsMock.Setup(r => r.GetById(It.IsAny<Guid>(),It.IsAny<ITransaction>())).
                Returns(repo);
            _reps = _repsMock.Object;
            _servicesMapper = new NugetServicesMapper(_reps, _properties);
        }

        [TestMethod]
        public void ISPToQueryRemote()
        {
            var target = new V3_Query(Guid.NewGuid(),_nugetService, _properties, _reps, _servicesMapper,
                "/{repo}/v3/query")
            {
                RequestData = (a, b) => HandleRequest("ISPToQuery", a, b)
            };

            var serializableRequest = new SerializableRequest
            {
                Log = true,
                Protocol = "http",
                Url = "/nuget.org/v3/query",
                Host = "localhost:9080",
                PathParams = new Dictionary<string, string>
                {
                    {"repo","nuget.org" }
                }
            };

            var result = target.HandleRequest(serializableRequest);

            AreEquals<QueryResult>("ISTToQuery", result);
        }
    }
}
