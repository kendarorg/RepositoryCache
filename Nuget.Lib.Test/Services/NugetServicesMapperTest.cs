using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ioc;
using MultiRepositories.Repositories;
using Repositories;
using System.Collections.Generic;
using Moq;

namespace Nuget.Lib.Test
{
    [TestClass]
    public class NugetServicesMapperTest
    {
        private IRepositoryEntitiesRepository _repository;
        private Guid _repositoryId;

        [TestInitialize]
        public void Initialize()
        {
            var au = new AssemblyUtils();
            var data = au.ReadRes<NugetServicesMapperTest>("nuget.org.settings.json");

            _repositoryId = Guid.NewGuid();
            var mock = new Mock<IRepositoryEntitiesRepository>();
            mock.Setup(r => r.GetByType(It.IsAny<string>())).
                Returns(new List<RepositoryEntity>
                {
                    new RepositoryEntity
                    {
                        Address = "nuget.org",
                        Id = _repositoryId,
                        Mirror = true,
                        Prefix = "nuget.org",
                        Settings = data,
                        Type = "nuget"
                    }
                });
            _repository = mock.Object;
        }

        [TestMethod]
        public void ISPToLoadSimpleItemNoVersion()
        {
            var au = new AssemblyUtils();
            //
            var target = new NugetServicesMapper(_repository);
            target.Refresh();

            var packagePublish = target.From(_repositoryId, "PackagePublish/2.0.0");
            Assert.AreEqual("nuget.org/v2/publish", packagePublish);
        }

        [TestMethod]
        public void ISPToLoadRefItemNoVersion()
        {
            var au = new AssemblyUtils();
            var target = new NugetServicesMapper(_repository);
            target.Refresh();

            var packagePublish = target.From(_repositoryId, "PackageVersionDisplayMetadataUriTemplate");
            Assert.IsTrue(
                "nuget.org/v3/registrationsemver1" == packagePublish ||
                "nuget.org/v3/registrationsemver2" == packagePublish);
        }

        [TestMethod]
        public void ISPToLoadRefItemNullVersion()
        {
            var au = new AssemblyUtils();
            var target = new NugetServicesMapper(_repository);
            target.Refresh();

            var packagePublish = target.FromSemver(_repositoryId, "PackageVersionDisplayMetadataUriTemplate", null);
            Assert.AreEqual("nuget.org/v3/registrationsemver1", packagePublish);
        }


        [TestMethod]
        public void ISPToLoadRefItemOneVersion()
        {
            var au = new AssemblyUtils();

            var target = new NugetServicesMapper(_repository);
            target.Refresh();

            var packagePublish = target.FromSemver(_repositoryId, "PackageVersionDisplayMetadataUriTemplate", "1.0.0");
            Assert.AreEqual("nuget.org/v3/registrationsemver1", packagePublish);
        }


        [TestMethod]
        public void ISPToLoadRefItemFakeVersion()
        {
            var au = new AssemblyUtils();

            var target = new NugetServicesMapper(_repository);
            target.Refresh();

            var packagePublish = target.FromSemver(_repositoryId, "PackageVersionDisplayMetadataUriTemplate", "3.0.0");
            Assert.AreEqual("nuget.org/v3/registrationsemver1", packagePublish);
        }

        [TestMethod]
        public void ISPToLoadRefItemReferencedVersion()
        {
            var au = new AssemblyUtils();

            var target = new NugetServicesMapper(_repository);
            target.Refresh();

            var packagePublish = target.FromSemver(_repositoryId, "PackageVersionDisplayMetadataUriTemplate", "2.0.0");
            Assert.AreEqual("nuget.org/v3/registrationsemver2", packagePublish);
        }


        [TestMethod]
        public void ISPToLoadVisibleItems()
        {
            var au = new AssemblyUtils();

            var target = new NugetServicesMapper(_repository);
            target.Refresh();
            var visibles = target.GetVisibles(_repositoryId);

            Assert.AreEqual(21, visibles.Count);
        }
        
        [TestMethod]
        public void ISPToLoadFromNuget()
        {
            var au = new AssemblyUtils();
            //
            var target = new NugetServicesMapper(_repository);
            target.Refresh();

            var packagePublish = target.FromNuget(_repositoryId, "https://api.nuget.org/v3/registration3-gz/test/index.json");
            Assert.AreEqual("nuget.org/v3/registrationsemver1/test/index.json", packagePublish);
        }


        [TestMethod]
        public void ISPToLoadToNuget()
        {
            var au = new AssemblyUtils();
            //
            var target = new NugetServicesMapper(_repository);
            target.Refresh();

            var packagePublish = target.ToNuget(_repositoryId, "nuget.org/v3/registrationsemver1/test/index.json");
            Assert.AreEqual("https://api.nuget.org/v3/registration3-gz/test/index.json", packagePublish);
        }
    }
}
