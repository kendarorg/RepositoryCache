using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ioc;
using MultiRepositories.Repositories;
using Repositories;
using System.Collections.Generic;
using Moq;
using MultiRepositories;

namespace Nuget.Lib.Test
{
    [TestClass]
    public class NugetServicesMapperTest
    {
        private IRepositoryEntitiesRepository _repository;
        private AppProperties _properties;
        private Guid _repositoryId;

        [TestInitialize]
        public void Initialize()
        {
            var au = new AssemblyUtils();
            var data = au.ReadRes<NugetServicesMapperTest>("nuget.org.settings.json");
            _properties = new AppProperties(null,null);
            _repositoryId = Guid.NewGuid();
            
            var repositoryEntitiesRepository = new Mock<IRepositoryEntitiesRepository>();

            var repo = new RepositoryEntity
            {
                Address = "nuget.org",
                Id = _repositoryId,
                Mirror = true,
                Prefix = "nuget.org",
                Settings = data,
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
            _repository = repositoryEntitiesRepository.Object;
        }

        [TestMethod]
        public void ISPToLoadSimpleItemNoVersion()
        {
            var au = new AssemblyUtils();
            //
            var target = new NugetServicesMapper(_repository, _properties);
            target.Refresh();

            var packagePublish = target.From(_repositoryId, "PackagePublish/2.0.0");
            Assert.AreEqual("http://localhost:9080/nuget.org/v2/publish", packagePublish);
        }

        [TestMethod]
        public void ISPToLoadRefItemNoVersion()
        {
            var au = new AssemblyUtils();
            var target = new NugetServicesMapper(_repository,_properties);
            target.Refresh();

            var packagePublish = target.From(_repositoryId, "PackageVersionDisplayMetadataUriTemplate");
            Assert.IsTrue(
                "http://localhost:9080/nuget.org/v3/registrationsemver1" == packagePublish ||
                "http://localhost:9080/nuget.org/v3/registrationsemver2" == packagePublish);
        }

        [TestMethod]
        public void ISPToLoadRefItemNullVersion()
        {
            var au = new AssemblyUtils();
            var target = new NugetServicesMapper(_repository, _properties);
            target.Refresh();

            var packagePublish = target.FromSemver(_repositoryId, "PackageVersionDisplayMetadataUriTemplate", null);
            Assert.AreEqual("http://localhost:9080/nuget.org/v3/registrationsemver1", packagePublish);
        }


        [TestMethod]
        public void ISPToLoadRefItemOneVersion()
        {
            var au = new AssemblyUtils();

            var target = new NugetServicesMapper(_repository, _properties);
            target.Refresh();

            var packagePublish = target.FromSemver(_repositoryId, "PackageVersionDisplayMetadataUriTemplate", "1.0.0");
            Assert.AreEqual("http://localhost:9080/nuget.org/v3/registrationsemver1", packagePublish);
        }


        [TestMethod]
        public void ISPToLoadRefItemFakeVersion()
        {
            var au = new AssemblyUtils();

            var target = new NugetServicesMapper(_repository, _properties);
            target.Refresh();

            var packagePublish = target.FromSemver(_repositoryId, "PackageVersionDisplayMetadataUriTemplate", "3.0.0");
            Assert.AreEqual("http://localhost:9080/nuget.org/v3/registrationsemver1", packagePublish);
        }

        [TestMethod]
        public void ISPToLoadRefItemReferencedVersion()
        {
            var au = new AssemblyUtils();

            var target = new NugetServicesMapper(_repository, _properties);
            target.Refresh();

            var packagePublish = target.FromSemver(_repositoryId, "PackageVersionDisplayMetadataUriTemplate", "2.0.0");
            Assert.AreEqual("http://localhost:9080/nuget.org/v3/registrationsemver2", packagePublish);
        }


        [TestMethod]
        public void ISPToLoadVisibleItems()
        {
            var au = new AssemblyUtils();

            var target = new NugetServicesMapper(_repository, _properties);
            target.Refresh();
            var visibles = target.GetVisibles(_repositoryId);

            Assert.AreEqual(21, visibles.Count);
        }
        
        [TestMethod]
        public void ISPToLoadFromNuget()
        {
            var au = new AssemblyUtils();
            //
            var target = new NugetServicesMapper(_repository, _properties);
            target.Refresh();

            var packagePublish = target.FromNuget(_repositoryId, "https://api.nuget.org/v3/registration3-gz/test/index.json");
            Assert.AreEqual("http://localhost:9080/nuget.org/v3/registrationsemver1/test/index.json", packagePublish);
        }


        [TestMethod]
        public void ISPToLoadToNuget()
        {
            var au = new AssemblyUtils();
            //
            var target = new NugetServicesMapper(_repository, _properties);
            target.Refresh();

            var packagePublish = target.ToNuget(_repositoryId, "http://localhost:9080/nuget.org/v3/registrationsemver1/test/index.json");
            Assert.AreEqual("https://api.nuget.org/v3/registration3-gz/test/index.json", packagePublish);
        }
    }
}
