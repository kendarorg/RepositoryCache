using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using Maven.News;
using Maven.Services;
using MultiRepositories.Repositories;
using MavenProtocol;
using Repositories;
using MavenProtocol.News;
using System.Text.RegularExpressions;
using MavenProtocol.Apis;
using Moq;

namespace Maven.Lib.Test.Api
{
    /// <summary>
    /// Summary description for ArtifactsApiTest
    /// </summary>
    [TestFixture]
    public class ArtifactsApiTest
    {
        public ArtifactsApiTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;
        private ArtifactsApi _target;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private Mock<IArtifactsRepository> _artifactsRepositoryMock;

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use SetUp to run code before running each test 

        IArtifactsRepository _artifactsRepository = null;
        private Mock<IRepositoryEntitiesRepository> _repositoriesRepositoryMock;
        IRepositoryEntitiesRepository _repositoriesRepository = null;
        IArtifactsStorage _artifactsStorage = null;
        Mock<IArtifactsStorage> _artifactsStorageMock = null;
        IHashCalculator _hashCalculator = null;
        Mock<IHashCalculator> _hashCalculatorMock = null;
        ITransactionManager _transactionManager = null;

        IReleaseArtifactRepository _releaseArtifactRepository = null;
        Mock<IReleaseArtifactRepository> _releaseArtifactRepositoryMock = null;
        IPomApi _pomApi = null;
        Mock<IPomApi> _pomApiMock = null;
        IMetadataApi _metadataApi = null;
        Mock<IMetadataApi> _metadataApiMock = null;
        IServicesMapper _servicesMapper = null;
        Mock<IServicesMapper> _servicesMapperMock = null;
        [SetUp]
        public void MyTestInitialize()
        {

            _artifactsRepositoryMock = new Mock<IArtifactsRepository>();
            _artifactsRepository = _artifactsRepositoryMock.Object;
            _repositoriesRepositoryMock = new Mock<IRepositoryEntitiesRepository>();
            _repositoriesRepository = _repositoriesRepositoryMock.Object;
            _artifactsStorageMock = new Mock<IArtifactsStorage>();
            _artifactsStorage = _artifactsStorageMock.Object;

            _hashCalculatorMock = new Mock<IHashCalculator>();
            _hashCalculator = _hashCalculatorMock.Object;
            var transactionManagerMock = new Mock<ITransactionManager>();
            transactionManagerMock.Setup(a => a.BeginTransaction()).Returns(new Mock<ITransaction>().Object);
            _transactionManager = transactionManagerMock.Object;
            _releaseArtifactRepositoryMock = new Mock<IReleaseArtifactRepository>();
            _releaseArtifactRepository = _releaseArtifactRepositoryMock.Object;
            _pomApiMock = new Mock<IPomApi>();
            _pomApi = _pomApiMock.Object;
            _metadataApiMock = new Mock<IMetadataApi>();
            _metadataApi = _metadataApiMock.Object;

            _servicesMapperMock = new Mock<IServicesMapper>();
            _servicesMapper = _servicesMapperMock.Object;



            _target = new ArtifactsApi(
                _artifactsRepository, _artifactsStorage, _repositoriesRepository, _servicesMapper,
                _hashCalculator, _transactionManager, _releaseArtifactRepository,
                _pomApi, _metadataApi);

        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        #region CAN_HANDLE
        [Test]
        public void ITSPToHandleStandardRequests()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "ear"
            };
            Assert.IsTrue(_target.CanHandle(mi));
        }
        [Test]
        public void ITSNPToHandlePomRequests()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "pom",
            };
            Assert.IsFalse(_target.CanHandle(mi));
        }
        [Test]
        public void ITSNPToHandleMEtaRequests()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "war",
                Meta = "metadata.xml"
            };
            Assert.IsFalse(_target.CanHandle(mi));
        }
        #endregion


        [Test]
        public void ISNBPToRetrieveNotExistingPackage()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar"
            };
            var result = _target.Retrieve(mi);
            Assert.IsNull(result);
        }
        [Test]
        public void ISBPToRetrievePackage()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                RepoId = Guid.NewGuid()
            };

            var artifactVersion = new ArtifactEntity();
            _artifactsRepositoryMock.Setup(a => a.GetSingleArtifact(
                It.Is<Guid>(b => b == mi.RepoId), It.IsAny<string[]>(), It.IsAny<string>(),
                It.Is<string>(b => b == mi.Version), It.IsAny<string>(), It.Is<string>(b => b == mi.Extension),
                It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<string>())).
                Returns(artifactVersion);

            var result = _target.Retrieve(mi);

            _artifactsStorageMock.Verify(a => a.Load(It.IsAny<RepositoryEntity>(), It.IsAny<MavenIndex>()), Times.Exactly(1));
            _artifactsRepositoryMock.Verify(a => a.GetSingleArtifact(
                It.Is<Guid>(b => b == mi.RepoId), It.IsAny<string[]>(), It.IsAny<string>(),
                It.Is<string>(b => b == mi.Version), It.IsAny<string>(), It.Is<string>(b => b == mi.Extension),
                It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Exactly(1));
            Assert.IsNotNull(result);
        }


        [Test]
        public void ISBPToGeneratePackageLocally()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                RepoId = Guid.NewGuid(),
                Content = new byte[] { },
                Group = new string[] { "org", "test" }
            };



            var result = _target.Generate(mi, false);

            _releaseArtifactRepositoryMock.Verify(a => a.Save(It.IsAny<ReleaseVersion>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _artifactsStorageMock.Verify(a => a.Save(It.IsAny<RepositoryEntity>(), It.IsAny<MavenIndex>(), It.IsAny<byte[]>()), Times.Exactly(1));
            _artifactsRepositoryMock.Verify(a => a.Save(It.IsAny<ArtifactEntity>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _pomApiMock.Verify(a => a.Generate(It.IsAny<MavenIndex>(), It.IsAny<bool>()), Times.Exactly(0));
            _pomApiMock.Verify(a => a.UpdateClassifiers(It.IsAny<MavenIndex>()), Times.Exactly(1));
            _metadataApiMock.Verify(a => a.GenerateNoSnapshot(It.IsAny<MavenIndex>()), Times.Exactly(1));

            Assert.IsNotNull(result);
        }

        [Test]
        public void ISBPToGeneratePackageLocallyChecksum()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                Checksum = "md5",
                RepoId = Guid.NewGuid(),
                Content = new byte[] { },
                Group = new string[] { "org", "test" }
            };



            var result = _target.Generate(mi, false);

            _releaseArtifactRepositoryMock.Verify(a => a.Save(It.IsAny<ReleaseVersion>(), It.IsAny<ITransaction>()), Times.Exactly(0));
            _artifactsStorageMock.Verify(a => a.Save(It.IsAny<RepositoryEntity>(), It.IsAny<MavenIndex>(), It.IsAny<byte[]>()), Times.Exactly(0));
            _artifactsRepositoryMock.Verify(a => a.Save(It.IsAny<ArtifactEntity>(), It.IsAny<ITransaction>()), Times.Exactly(0));
            _pomApiMock.Verify(a => a.Generate(It.IsAny<MavenIndex>(), It.IsAny<bool>()), Times.Exactly(0));
            _pomApiMock.Verify(a => a.UpdateClassifiers(It.IsAny<MavenIndex>()), Times.Exactly(0));
            _metadataApiMock.Verify(a => a.GenerateNoSnapshot(It.IsAny<MavenIndex>()), Times.Exactly(0));

            Assert.IsNull(result);
        }



        [Test]
        public void ISBPToGenerateSnapshotPackageLocally()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                IsSnapshot = true,
                RepoId = Guid.NewGuid(),
                Content = new byte[] { },
                Group = new string[] { "org", "test" }
            };



            var result = _target.Generate(mi, false);

            _releaseArtifactRepositoryMock.Verify(a => a.Save(It.IsAny<ReleaseVersion>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _artifactsStorageMock.Verify(a => a.Save(It.IsAny<RepositoryEntity>(), It.IsAny<MavenIndex>(), It.IsAny<byte[]>()), Times.Exactly(1));
            _artifactsRepositoryMock.Verify(a => a.Save(It.IsAny<ArtifactEntity>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _pomApiMock.Verify(a => a.Generate(It.IsAny<MavenIndex>(), It.IsAny<bool>()), Times.Exactly(0));
            _pomApiMock.Verify(a => a.UpdateClassifiers(It.IsAny<MavenIndex>()), Times.Exactly(1));
            _metadataApiMock.Verify(a => a.GenerateNoSnapshot(It.IsAny<MavenIndex>()), Times.Exactly(0));

            Assert.IsNotNull(result);
        }


        [Test]
        public void ISBPToGeneratePackageRemote()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                RepoId = Guid.NewGuid(),
                Content = new byte[] { },
                Group = new string[] { "org", "test" }
            };

            var result = _target.Generate(mi, true);

            _releaseArtifactRepositoryMock.Verify(a => a.Save(It.IsAny<ReleaseVersion>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _artifactsStorageMock.Verify(a => a.Save(It.IsAny<RepositoryEntity>(), It.IsAny<MavenIndex>(), It.IsAny<byte[]>()), Times.Exactly(1));
            _artifactsRepositoryMock.Verify(a => a.Save(It.IsAny<ArtifactEntity>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _pomApiMock.Verify(a => a.Generate(It.IsAny<MavenIndex>(), It.IsAny<bool>()), Times.Exactly(1));
            _pomApiMock.Verify(a => a.UpdateClassifiers(It.IsAny<MavenIndex>()), Times.Exactly(1));
            _metadataApiMock.Verify(a => a.GenerateNoSnapshot(It.IsAny<MavenIndex>()), Times.Exactly(1));

            Assert.IsNotNull(result);
        }

        [Test]
        public void ISBPToUpdateRemotePackage()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                RepoId = Guid.NewGuid(),
                Content = new byte[] { },
                Group = new string[] { "org", "test" }
            };
            var artifactVersion = new ArtifactEntity();
            _artifactsRepositoryMock.Setup(a => a.GetSingleArtifact(
                It.Is<Guid>(b => b == mi.RepoId), It.IsAny<string[]>(), It.IsAny<string>(),
                It.Is<string>(b => b == mi.Version), It.IsAny<string>(), It.Is<string>(b => b == mi.Extension),
                It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<string>())).
                Returns(artifactVersion);

            var result = _target.Generate(mi, true);

            _releaseArtifactRepositoryMock.Verify(a => a.Save(It.IsAny<ReleaseVersion>(), It.IsAny<ITransaction>()), Times.Exactly(0));
            _artifactsStorageMock.Verify(a => a.Save(It.IsAny<RepositoryEntity>(), It.IsAny<MavenIndex>(), It.IsAny<byte[]>()), Times.Exactly(1));
            _artifactsRepositoryMock.Verify(a => a.Save(It.IsAny<ArtifactEntity>(), It.IsAny<ITransaction>()), Times.Exactly(0));
            _pomApiMock.Verify(a => a.Generate(It.IsAny<MavenIndex>(), It.IsAny<bool>()), Times.Exactly(0));
            _pomApiMock.Verify(a => a.UpdateClassifiers(It.IsAny<MavenIndex>()), Times.Exactly(0));
            _metadataApiMock.Verify(a => a.GenerateNoSnapshot(It.IsAny<MavenIndex>()), Times.Exactly(0));

            Assert.IsNotNull(result);
        }


        [Test]
        public void ISBPToUpdateRemotePackageChecksum()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                Checksum = "md5",
                RepoId = Guid.NewGuid(),
                Content = new byte[] { },
                Group = new string[] { "org", "test" }
            };
            var artifactVersion = new ArtifactEntity();
            _artifactsRepositoryMock.Setup(a => a.GetSingleArtifact(
                It.Is<Guid>(b => b == mi.RepoId), It.IsAny<string[]>(), It.IsAny<string>(),
                It.Is<string>(b => b == mi.Version), It.IsAny<string>(), It.Is<string>(b => b == mi.Extension),
                It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<string>())).
                Returns(artifactVersion);

            var result = _target.Generate(mi, true);

            _releaseArtifactRepositoryMock.Verify(a => a.Save(It.IsAny<ReleaseVersion>(), It.IsAny<ITransaction>()), Times.Exactly(0));
            _artifactsStorageMock.Verify(a => a.Save(It.IsAny<RepositoryEntity>(), It.IsAny<MavenIndex>(), It.IsAny<byte[]>()), Times.Exactly(0));
            _artifactsRepositoryMock.Verify(a => a.Save(It.IsAny<ArtifactEntity>(), It.IsAny<ITransaction>()), Times.Exactly(0));
            _pomApiMock.Verify(a => a.Generate(It.IsAny<MavenIndex>(), It.IsAny<bool>()), Times.Exactly(0));
            _pomApiMock.Verify(a => a.UpdateClassifiers(It.IsAny<MavenIndex>()), Times.Exactly(0));
            _metadataApiMock.Verify(a => a.GenerateNoSnapshot(It.IsAny<MavenIndex>()), Times.Exactly(0));

            Assert.IsNotNull(result);
        }

        [Test]
        public void ISNBPToUpdateTimestampedSnapshot()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                RepoId = Guid.NewGuid(),
                IsSnapshot = true,
                Content = new byte[] { },
                Group = new string[] { "org", "test" }
            };
            _servicesMapperMock.Setup(a => a.HasTimestampedSnapshot(It.IsAny<Guid>())).Returns(true);
            var artifactVersion = new ArtifactEntity();
            _artifactsRepositoryMock.Setup(a => a.GetSingleArtifact(
                It.Is<Guid>(b => b == mi.RepoId), It.IsAny<string[]>(), It.IsAny<string>(),
                It.Is<string>(b => b == mi.Version), It.IsAny<string>(), It.Is<string>(b => b == mi.Extension),
                It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<string>())).
                Returns(artifactVersion);

            Assert.Throws<Exception>(() => _target.Generate(mi, false));
        }


        [Test]
        public void ISNBPToUpdateRelease()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                RepoId = Guid.NewGuid(),
                Content = new byte[] { },
                Group = new string[] { "org", "test" }
            };

            var artifactVersion = new ArtifactEntity();
            _artifactsRepositoryMock.Setup(a => a.GetSingleArtifact(
                It.Is<Guid>(b => b == mi.RepoId), It.IsAny<string[]>(), It.IsAny<string>(),
                It.Is<string>(b => b == mi.Version), It.IsAny<string>(), It.Is<string>(b => b == mi.Extension),
                It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<string>())).
                Returns(artifactVersion);

            Assert.Throws<Exception>(() => _target.Generate(mi, false));
        }

        [Test]
        public void ISBPToUpdateFixedSnapshot()
        {
            var mi = new MavenIndex
            {
                Version = "1",
                Extension = "jar",
                RepoId = Guid.NewGuid(),
                IsSnapshot = true,
                Build = "2",
                ArtifactId = "test",
                Content = new byte[] { },
                Group = new string[] { "org", "test" }
            };
            _servicesMapperMock.Setup(a => a.HasTimestampedSnapshot(It.IsAny<Guid>())).Returns(false);
            var artifactVersion = new ArtifactEntity
            {
                Version = "1",
                Build = "1",
                IsSnapshot = true
            };
            ReleaseVersion savedRelease = null;
            _artifactsRepositoryMock.Setup(a => a.GetSingleArtifact(
                It.Is<Guid>(b => b == mi.RepoId), It.IsAny<string[]>(), It.IsAny<string>(),
                It.Is<string>(b => b == mi.Version), It.IsAny<string>(), It.Is<string>(b => b == mi.Extension),
                It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<string>())).
                Returns(artifactVersion);

            _releaseArtifactRepositoryMock.Setup(a => a.GetForArtifact(
                It.Is<Guid>(b => b == mi.RepoId), It.IsAny<string[]>(), It.IsAny<string>(), It.IsAny<bool>())).
                Returns(new ReleaseVersion
                {
                    Version = artifactVersion.Version,
                    Build = artifactVersion.Build
                });
            _releaseArtifactRepositoryMock.Setup(a => a.Save(It.IsAny<ReleaseVersion>(), It.IsAny<ITransaction>())).Callback((ReleaseVersion b, ITransaction c) =>
            {
                savedRelease = b;
            });

            var result = _target.Generate(mi, false);

            _releaseArtifactRepositoryMock.Verify(a => a.Save(It.IsAny<ReleaseVersion>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _artifactsStorageMock.Verify(a => a.Save(It.IsAny<RepositoryEntity>(), It.IsAny<MavenIndex>(), It.IsAny<byte[]>()), Times.Exactly(1));
            _artifactsRepositoryMock.Verify(a => a.Save(It.IsAny<ArtifactEntity>(), It.IsAny<ITransaction>()), Times.Exactly(1));
            _pomApiMock.Verify(a => a.Generate(It.IsAny<MavenIndex>(), It.IsAny<bool>()), Times.Exactly(0));
            _pomApiMock.Verify(a => a.UpdateClassifiers(It.IsAny<MavenIndex>()), Times.Exactly(1));
            _metadataApiMock.Verify(a => a.GenerateNoSnapshot(It.IsAny<MavenIndex>()), Times.Exactly(0));

            Assert.IsNotNull(result);
            Assert.IsNotNull(savedRelease);
            Assert.AreEqual(artifactVersion.Build, savedRelease.Build);
        }

        [Test]
        public void ISBPToGeneratePomWhenRemote()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void ISBPToChangeTheReleaseArtifactsWhenAddingMoreRecentVersion()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void ISBPToChangeTheTimestampedSnapshotArtifactsWhenAddingMoreRecentVersion()
        {
            Assert.Inconclusive();
        }


    }



}
