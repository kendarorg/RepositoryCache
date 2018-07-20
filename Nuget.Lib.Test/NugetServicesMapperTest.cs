using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ioc;
using MultiRepositories.Repositories;
using Repositories;
using System.Collections.Generic;

namespace Nuget.Lib.Test
{
    public class MockRepo : IRepositoryEntitiesRepository
    {
        public long Count => throw new NotImplementedException();

        public void Clean(ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RepositoryEntity> GetAll(ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public RepositoryEntity GetById(Guid id, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public RepositoryEntity GetByName(string repoPrefix)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public int Save(RepositoryEntity be, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public int Update(RepositoryEntity be, ITransaction transaction = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RepositoryEntity> GetByType(string type)
        {
            var au = new AssemblyUtils();
            var data = au.ReadRes<NugetServicesMapperTest>("nuget.org.settings.json");
            yield return new RepositoryEntity
            {
                Address = "nuget.org",
                Id = Id,
                Mirror = true,
                Prefix = "nuget.org",
                Settings = data,
                Type = "nuget"
            };
        }

        public Guid Id = Guid.NewGuid();
    }
    [TestClass]
    public class NugetServicesMapperTest
    {
        [TestMethod]
        public void ISPToLoadSimpleItemNoVersion()
        {
            var au = new AssemblyUtils();
            var repo = new MockRepo();
            var target = new NugetServicesMapper(repo);
            target.Refresh();

            var packagePublish = target.From(repo.Id, "PackagePublish/2.0.0");
            Assert.AreEqual("nuget.org/v2/publish", packagePublish);
        }

        [TestMethod]
        public void ISPToLoadRefItemNoVersion()
        {
            var au = new AssemblyUtils();
            var repo = new MockRepo();
            var target = new NugetServicesMapper(repo);
            target.Refresh();

            var packagePublish = target.From(repo.Id, "PackageVersionDisplayMetadataUriTemplate");
            Assert.IsTrue(
                "nuget.org/v3/registrationsemver1" == packagePublish ||
                "nuget.org/v3/registrationsemver2" == packagePublish);
        }

        [TestMethod]
        public void ISPToLoadRefItemNullVersion()
        {
            var au = new AssemblyUtils();
            var repo = new MockRepo();
            var target = new NugetServicesMapper(repo);
            target.Refresh();

            var packagePublish = target.FromSemver(repo.Id, "PackageVersionDisplayMetadataUriTemplate",null);
            Assert.AreEqual("nuget.org/v3/registrationsemver1",packagePublish);
        }


        [TestMethod]
        public void ISPToLoadRefItemOneVersion()
        {
            var au = new AssemblyUtils();
            var repo = new MockRepo();
            var target = new NugetServicesMapper(repo);
            target.Refresh();

            var packagePublish = target.FromSemver(repo.Id, "PackageVersionDisplayMetadataUriTemplate", "1.0.0");
            Assert.AreEqual("nuget.org/v3/registrationsemver1", packagePublish);
        }


        [TestMethod]
        public void ISPToLoadRefItemFakeVersion()
        {
            var au = new AssemblyUtils();
            var repo = new MockRepo();
            var target = new NugetServicesMapper(repo);
            target.Refresh();

            var packagePublish = target.FromSemver(repo.Id, "PackageVersionDisplayMetadataUriTemplate", "3.0.0");
            Assert.AreEqual("nuget.org/v3/registrationsemver1", packagePublish);
        }

        [TestMethod]
        public void ISPToLoadRefItemReferencedVersion()
        {
            var au = new AssemblyUtils();
            var repo = new MockRepo();
            var target = new NugetServicesMapper(repo);
            target.Refresh();

            var packagePublish = target.FromSemver(repo.Id, "PackageVersionDisplayMetadataUriTemplate", "2.0.0");
            Assert.AreEqual("nuget.org/v3/registrationsemver2", packagePublish);
        }


        [TestMethod]
        public void ISPToLoadVisibleItems()
        {
            var au = new AssemblyUtils();
            var repo = new MockRepo();
            var target = new NugetServicesMapper(repo);
            target.Refresh();
            var visibles = target.GetVisibles(repo.Id);

            Assert.AreEqual(21, visibles.Count);
        }
    }
}
